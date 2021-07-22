using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class GameLogic : MonoBehaviour
{
    public GameObject MainMenuObj, OptionsMenuObj;

    public List<int> playersID;
    
    public List<GameObject> players;

    private int playerToMove = 0;
    
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
        JObject newGameState = new JObject();
        newGameState.Add("view", new JObject());
        Debug.Log(code);
        AirConsole.instance.SetCustomDeviceState(newGameState);
        SetView("menuNavigation");

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
        if (data["action"].ToString().Equals("mainMenuStart"))
        {
            MainMenuObj.GetComponent<MainMenu>().PlayGame();
            SetView("mainGame");
            //Dice1 = GameObject.Find("Canvas/Dice1").GetComponent<Text>();
            //Dice2 = GameObject.Find("Canvas/Dice2").GetComponent<Text>();
        } 
        else if (data["action"].ToString().Equals("mainMenuOptions"))
        {
            MainMenuObj.SetActive(false);
            OptionsMenuObj.SetActive(true);
            MainMenuObj.GetComponent<MainMenu>().SetOptionsMenuSelection();
            SetView("optionsNavigation");
        }
        else if (data["action"].ToString().Equals("mainMenuExit"))
        {
            MainMenuObj.GetComponent<MainMenu>().ExitGame();
        }
        else if (data["action"].ToString().Equals("optionsMenuBack"))
        {
            MainMenuObj.SetActive(true);
            OptionsMenuObj.SetActive(false);
            MainMenuObj.GetComponent<MainMenu>().SetMainMenuSelection();
            SetView("menuNavigation");
        }
        else if (data["action"].ToString().Equals("rollDice"))
        {
            
            
        }
    }

    public void MovePlayer(int positions)
    {
        players[0].GetComponent<PlayerScript>().DiceRoll(positions);
    }

    private void OnConnect(int device_id)
    {
        playersID.Add(device_id);
    }

    public void SetView(string viewName)
    {
        AirConsole.instance.SetCustomDeviceStateProperty("view", viewName);
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
