using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class GameLogic : MonoBehaviour
{
    public GameObject MainMenuObj;

    public List<int> playersID;
    
    public List<GameObject> players;

    private int playerToMove = 0;

    private int playersReady = 0;

    public DiceScript dice;
    
    private void Start()
    {
        playersID = new List<int>();
        players = new List<GameObject>();
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
            var newView = new { view = "playerMoving" };
            AirConsole.instance.Message(fromDeviceID,newView);
            dice.OnMouseDown();
        }
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
}
