using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using Cinemachine;

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
    public GameObject mainGameUI;
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
    private static readonly string[] miniGamesObjNames = {
        "JoKenPoScript"
    };
    
    private void Start()
    {
        playersIDs = new List<int>();
        List<int> playersReady = GameObject.Find("MenuLogic").GetComponent<MenuLogic>().GetPlayersReady();
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

    public void Duel(int challenger, int defender) 
    {
        diceObj.SetActive(false);
        cinemachineCamera.Follow = null;
        curMiniGame = Random.Range(0, miniGamesAmount);
        mainGameUI.SetActive(false);
        miniGamesRules[curMiniGame].SetActive(true);
        curChallenger = playersIDs[challenger];
        curDefender = playersIDs[defender];
        SetDeviceView(curChallenger, "gameRules");
        SetDeviceView(curDefender, "gameRules");
        StartCoroutine(LoadMiniGame(10f));
    }

    IEnumerator LoadMiniGame(float timeToWait)
    {
        cameraObj.transform.position = new Vector3(0f, -100f, -10f);
        SceneManager.LoadSceneAsync("MiniGame0", LoadSceneMode.Additive);
        yield return new WaitForSeconds(timeToWait);
        cameraObj.transform.position = GameObject.Find("CameraPosition").transform.position;
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
        cameraObj.transform.position = new Vector3(0,0,-10f);
        mainGameUI.SetActive(true);
        SceneManager.UnloadSceneAsync(sceneName);
        if (curWinnerPlayer == -1)
        {
            for (int i = 0; i < playersIDs.Count; i++)
            {
                if (playersIDs[i] == curChallenger)
                {
                    //ativar animação // eperar animação terminar
                    cinemachineCamera.Follow = playersScripts[i].transform;
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
                    this.isGameOver = playersScripts[i].GetComponent<PlayerScript>().getGameOver();
                    Debug.Log("Terminou o jogo = " + this.isGameOver + " Vidas do Player = " + playersScripts[i].GetComponent<PlayerScript>().getLifes());
                    break;
                }    
            }
            EndMove();
        }
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
}
