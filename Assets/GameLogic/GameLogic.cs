using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class GameLogic : MonoBehaviour
{
    public GameObject MainMenuObj, OptionsMenuObj;
    private Text Dice1, Dice2;
    private int player1, player2;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Awake()
    {
        //menuRef.GetComponent
        //register all the events I need 
        AirConsole.instance.onReady += OnReady;
        AirConsole.instance.onMessage += OnMessage;

        //no device state can be set until AirConsole is ready, so I disable the buttons until then
        //gameStateButtons = FindObjectsOfType<Button>();
        //for (int i = 0; i < gameStateButtons.Length; ++i)
        //{
        //    gameStateButtons[i].interactable = false;
        //}
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
            player1 = fromDeviceID;
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
            if (Dice1 == null)
            {
                Dice1 = GameObject.Find("Canvas/Dice1").GetComponent<Text>();
                Dice2 = GameObject.Find("Canvas/Dice2").GetComponent<Text>();
            }
            if (fromDeviceID == player1)
            {
                Dice1.text = Random.Range(1,7).ToString();
            }
            else
            {
                Dice2.text = Random.Range(1, 7).ToString();
            }
        }
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
        }
    }
}
