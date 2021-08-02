using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class GameLogic : MonoBehaviour
{
    public List<GameObject> miniGames;
    public JoKenPoScript joKenPo;
    public GameObject cameraObj;
    public List<int> playersID;
    public List<GameObject> players;
    private int playerToMove = 0;
    private int playersReady = 0;
    private int randomGame = 0;
    private int curChallenger, curDefender, whoWins = 0;
    public DiceScript dice;
    
    private void Start()
    {
        playersID = new List<int>();
        players = new List<GameObject>();
        miniGames = new List<GameObject>();
        DontDestroyOnLoad(gameObject);
    }

    void Awake()
    {
        AirConsole.instance.onReady += OnReady;
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;
    }

    void OnReady(string code)
    {
        //Initialize Game State
        //JObject newGameState = new JObject();
        //newGameState.Add("view", new JObject());
        //AirConsole.instance.SetCustomDeviceState(newGameState);
        //var newView = new { view = "menuNavigation" };
        //foreach (int id in playersID)
        //{
        //    AirConsole.instance.Message(id,newView);
        //}
        
        //now that AirConsole is ready, the buttons can be enabled 
        //for (int i = 0; i < gameStateButtons.Length; ++i)
        //{
        //    gameStateButtons[i].interactable = true;
        //}
    }

    void OnMessage(int fromDeviceID, JToken data)
    {
        Debug.Log("Message from " + fromDeviceID + ", data: " + data);
        if (data["action"] == null) return;
        if (data["action"].ToString().Equals("playerReady"))
        {
            playersReady += 1;
            if (playersReady == playersID.Count)
            {
                SceneManager.LoadScene("MainGameScene");
                for (int i = 0; i < playersID.Count; i++)
                {
                    if (i == playerToMove)
                    {
                        var newView = new { view = "diceDisplay" };
                        AirConsole.instance.Message(playersID[i], newView);
                    }
                    else
                    {
                        var newView = new { view = "notYourTurn" };
                        AirConsole.instance.Message(playersID[i], newView);
                    }
                }
            } 
            else
            {
                var newView = new { view = "waitingGame" };
                AirConsole.instance.Message(fromDeviceID, newView);
            }
        } 
        else if (data["action"].ToString().Equals("rollDice"))
        {
            // miniGames[0].SetActive(true);
            // StartCoroutine(Tiltei());
            var newView = new { view = "playerMoving" };
            AirConsole.instance.Message(fromDeviceID,newView);
            dice.OnMouseDown();
            // miniGames[0].SetActive(false);
        }
        else if (data["JoKenPoMove"] != null)
        {
            joKenPo.ReceivePlayerChoice(fromDeviceID, data["JoKenPoMove"].ToString());
        }
    }

    IEnumerator Tiltei()
    {
        miniGames[0].SetActive(true);
        yield return new WaitForSeconds(2f);
        miniGames[0].SetActive(false);
    }

    public void MovePlayer(int positions)
    {
        players[playerToMove].GetComponent<PlayerScript>().DiceRoll(positions);
    }

    public void EndMove()
    {
        var newView = new { view = "notYourTurn" };
        AirConsole.instance.Message(playersID[playerToMove], newView);
        playerToMove = (playerToMove + 1) % players.Count;
        newView = new { view = "diceDisplay" };
        AirConsole.instance.Message(playersID[playerToMove], newView);
        dice.SetCoroutine(true);
    }

    private void OnConnect(int device_id)
    {
        playersID.Add(device_id);
        var newView = new { view = "menuNavigation" };
        AirConsole.instance.Message(device_id, newView);
    }

    public void SetStateProperty(string property, string value)
    {
        AirConsole.instance.SetCustomDeviceStateProperty(property, value);
    }

    void OnDestroy()
    {
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onReady -= OnReady;
            AirConsole.instance.onMessage -= OnMessage;
            AirConsole.instance.onConnect -= OnConnect;
        }
    }

    public void Duel(int challenger, int defender) 
    {
        randomGame = Random.Range(0, miniGames.Count);
        miniGames[randomGame].SetActive(true);
        curChallenger = playersID[challenger];
        curDefender = playersID[defender];
        SetDeviceView(curChallenger, "gameRules");
        SetDeviceView(curDefender, "gameRules");
        StartCoroutine(LoadMiniGame(10f));
    }

    IEnumerator LoadMiniGame(float timeToWait)
    {
        cameraObj.transform.position = new Vector3(0,-10f,-10f); // change to correct position
        SceneManager.LoadSceneAsync("MiniGame0", LoadSceneMode.Additive);
        yield return new WaitForSeconds(timeToWait);
        miniGames[randomGame].SetActive(false);
        joKenPo = GameObject.Find("JoKenPoScript").GetComponent<JoKenPoScript>();
        joKenPo.SetUIActive();
        joKenPo.SetPlayers(curChallenger, curDefender);
        SetDeviceView(curChallenger, "joKenPoOptions");
        SetDeviceView(curDefender, "joKenPoOptions");
    }

    public void SetDeviceView(int deviceID, string viewName)
    {
        var newView = new { view = viewName };
        AirConsole.instance.Message(deviceID, newView);
    }

    public void UnloadMiniGame(string sceneName)
    {
        cameraObj.transform.position = new Vector3(0,0,-10f);
        SceneManager.UnloadSceneAsync(sceneName);
        if (whoWins == -1)
        {
            for (int i = 0; i < playersID.Count; i++)
            {
                if (playersID[i] == curChallenger)
                {
                    players[i].GetComponent<PlayerScript>().ChangeBoardPositionLeader();
                    break;
                }    
            }
        }
        else if (whoWins == 1) {
            for (int i = 0; i < playersID.Count; i++)
            {
                if (playersID[i] == curChallenger)
                {
                    players[i].GetComponent<PlayerScript>().setMoveBack();
                    break;
                }    
            }
        }
        EndMove();
    }

    public void SetWhoWins(int winner)
    {
        whoWins = winner;
    }

    public void SetMiniGamePlayersView(string viewName)
    {
        SetDeviceView(curChallenger, viewName);
        SetDeviceView(curDefender, viewName);
    }
}
