using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine.Audio;
using Cinemachine;

public enum Characters {Hawy, Fiona};

[System.Serializable]
public class WaypointsArray
{
    public GameObject[] waypoints;
};

public class GameLogic : MonoBehaviour
{
    public DiceScript dice;
    public GameObject diceObj;
    public GameObject cameraObj;
    public CinemachineVirtualCamera cinemachineCamera;
    public AudioSource music;
    public GameObject mainGameUI;
    [SerializeField]
    private AudioClip win_challengerSFX = null;
    [SerializeField]
    private AudioClip lose_challengerSFX = null;
    [SerializeField]
    private AudioClip walkingSFX = null;
    [SerializeField]
    private AudioSource sfxSource;
    private LevelTransitionScript transitionScript;
    private bool isGameOver = false;
    private int playerToMove = 0;
    private int curMiniGame = 0;
    private int curChallenger = -1;
    private int curDefender = -1;
    private int curWinnerPlayer = -1;
    [SerializeField]
    private static int miniGamesAmount = 1;
    [SerializeField]
    private GameObject[] miniGamesRules;
    [SerializeField]
    private IMiniGameScript[] miniGamesScripts = new IMiniGameScript[miniGamesAmount];
    private List<GameObject> playersScripts;
    private List<int> playersIDs;
    [SerializeField]
    private GameObject[] playerPrefabs;
    [SerializeField]
    private Transform[] playerInitialPositions;
    [SerializeField]
    private static int boardSize = 6;
    [SerializeField]
    private GameObject[] boardPlaces;
    public WaypointsArray[] waypointsArray;
    private Characters gameWinner;
    private static readonly string[] miniGamesObjNames = {
        "JoKenPoScript"
    };
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        playersIDs = new List<int>();
        List<int> playersReady = GameObject.Find("MenuLogic").GetComponent<MenuLogic>().GetPlayersReady();
        transitionScript = GameObject.Find("LevelTransition").GetComponent<LevelTransitionScript>();
        foreach (int id in playersReady)
        {
            playersIDs.Add(id);
        }
        Destroy(GameObject.Find("MenuLogic"));
        playersScripts = new List<GameObject>();
        for (int i = 0; i < playersIDs.Count; i++)
        {
            GameObject newPlayer = Instantiate(playerPrefabs[i], playerInitialPositions[i].position, Quaternion.identity);
            newPlayer.GetComponent<PlayerScript>().SetPlayerIndex(i);
            playersScripts.Add(newPlayer);
        }
        for (int i = 0; i < playersIDs.Count; i++)
        {
            if (i == playerToMove)
            {
                SetDeviceView(playersIDs[i], "diceDisplay");
            }
            else
            {
                SetDeviceView(playersIDs[i], "notYourTurn");
            }
        }
        CameraFocusOnPlayerToMove();
    }

    void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;
    }

    void OnMessage(int fromDeviceID, JToken data)
    {
        if (data["action"] == null) return;
        if (data["action"].ToString().Equals("rollDice"))
        {
            SetDeviceView(fromDeviceID, "playerMoving");
            dice.ThrowDice();
        }
        else if (data["fromMiniGame"] != null)
        {
            miniGamesScripts[curMiniGame].ReceivePlayerData(fromDeviceID, data);
        }
    }

    public void MovePlayer(int positions)
    {
        playersScripts[playerToMove].GetComponent<PlayerScript>().DiceRoll(positions);
    }

    public void EndMove()
    {
        SetDeviceView(playersIDs[playerToMove], "notYourTurn");
        playerToMove = (playerToMove + 1) % playersScripts.Count;
        SetDeviceView(playersIDs[playerToMove], "diceDisplay");
        dice.SetCoroutine(true);
    }

    void OnDestroy()
    {
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onMessage -= OnMessage;
        }
    }

    public void Duel(int challenger, int defender, Transform background) 
    {
        cinemachineCamera.Follow = null;
        StartCoroutine(StartMiniGame(challenger, defender, background));
    }

    IEnumerator StartMiniGame(int challenger, int defender, Transform background)
    {
        yield return new WaitForSeconds(1.0f);
        curMiniGame = Random.Range(0, miniGamesAmount);
        curChallenger = playersIDs[challenger];
        curDefender = playersIDs[defender];
        SetDeviceView(curChallenger, "gameRules");
        SetDeviceView(curDefender, "gameRules");
        StartCoroutine(LoadMiniGame(8f, background));
    }

    IEnumerator LoadMiniGame(float timeToWait, Transform background)
    {
        music.mute = true;
        transitionScript.LoadLevel("MiniGame0", true, 1f, LoadSceneMode.Additive);
        yield return new WaitForSeconds(1f);
        diceObj.SetActive(false);
        mainGameUI.SetActive(false);
        miniGamesRules[curMiniGame].SetActive(true);
        cameraObj.transform.position = new Vector3(0f, -1000f, -10f);
        yield return new WaitForSeconds(timeToWait);
        Transform cameraTransform = GameObject.Find("CameraPosition").transform;
        cameraTransform.position = background.position;
        cameraObj.transform.position = new Vector3(cameraTransform.position.x, cameraTransform.position.y, -10f);
        miniGamesRules[curMiniGame].SetActive(false);
        miniGamesScripts[curMiniGame] = GameObject.Find(miniGamesObjNames[curMiniGame]).GetComponent<IMiniGameScript>();
        miniGamesScripts[curMiniGame].SetUIActive();
        miniGamesScripts[curMiniGame].SetPlayers(curChallenger, curDefender);
        SetDeviceView(curChallenger, "joKenPoOptions", true);
        SetDeviceView(curDefender, "joKenPoOptions", false);
    }

    public void SetDeviceView(int deviceID, string viewName, bool isChallengerDevice = false)
    {
        var newView = new { view = viewName, isChallenger = isChallengerDevice };

        AirConsole.instance.Message(deviceID, newView);
    }

    public void UnloadMiniGame(string sceneName)
    {
        StartCoroutine(UnloadMiniGameCoroutine(sceneName));
    }
    IEnumerator UnloadMiniGameCoroutine(string sceneName)
    {
        transitionScript.FadeIn();
        yield return new WaitForSeconds(1f);
        transitionScript.FadeOut();
        cameraObj.transform.position = new Vector3(0,0,-10f);
        SceneManager.UnloadSceneAsync(sceneName);
        if (curWinnerPlayer == -1)
        {
            for (int i = 0; i < playersIDs.Count; i++)
            {
                if (playersIDs[i] == curChallenger)
                {
                    //ativar animação // eperar animação terminar
                    cinemachineCamera.Follow = playersScripts[i].transform;
                    sfxSource.PlayOneShot(win_challengerSFX, 0.5f);
                    playersScripts[i].GetComponent<PlayerScript>().setAttack();
                    //playersScripts[i].GetComponent<PlayerScript>().ChangeBoardPositionLeader();
                    
                    break;
                }    
            }
            EndMove();
            //CameraFocusOnPlayerToMove();
        }
        else if (curWinnerPlayer == 1) {
            for (int i = 0; i < playersIDs.Count; i++)
            {
                if (playersIDs[i] == curChallenger)
                {
                    cinemachineCamera.Follow = playersScripts[i].transform;
                    //playersScripts[i].GetComponent<PlayerScript>().setMoveBack();
                    playersScripts[i].GetComponent<PlayerScript>().setTakeDamage();
                    sfxSource.PlayOneShot(lose_challengerSFX, 0.5f);
                    this.isGameOver = playersScripts[i].GetComponent<PlayerScript>().getGameOver();
                    Debug.Log("Terminou o jogo = " + this.isGameOver + " Vidas do Player = " + playersScripts[i].GetComponent<PlayerScript>().getLifes());
                    break;
                }    
            }
            if (this.isGameOver)
            {
                Characters winner = Characters.Hawy;
                for (int i = 0; i < playersIDs.Count; i++)
                {
                    PlayerScript player = playersScripts[i].GetComponent<PlayerScript>();
                    if (player.getLifes() > 0)
                    {
                        winner = player.GetCharacther();
                        break;
                    }
                }
                gameWinner = winner;
                transitionScript.LoadLevel("GameEnding", true, 1f);
            }
            else 
            {
                EndMove();
            }
        }
        mainGameUI.SetActive(true);
        music.mute = false;
        diceObj.SetActive(true);
    }

    public void SetWhoWins(int winner)
    {
        curWinnerPlayer = winner;
    }

    public void SetMiniGamePlayersView(string viewName)
    {
        SetDeviceView(curChallenger, viewName);
        SetDeviceView(curDefender, viewName);
    }
    
    public GameObject[] GetBoardPlaces()
    {
        return this.boardPlaces;
    }

    public GameObject[] GetWaypoints(int playerIndex)
    {
        return waypointsArray[playerIndex].waypoints;
    }

    public void CameraFocusOnPlayerToMove()
    {
        cinemachineCamera.Follow = playersScripts[playerToMove].transform;
    }

    public Characters GetGameWinner()
    {
        return gameWinner;
    }
}
