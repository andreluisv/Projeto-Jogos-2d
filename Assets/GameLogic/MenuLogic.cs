using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class MenuLogic : MonoBehaviour
{
    private int playersConnected = 0;
    private int playerToMove = 0;
    private List<int> playersReady;
    public LevelTransitionScript levelTransition;
    
    private void Awake()
    {
        playersReady = new List<int>();
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;
        DontDestroyOnLoad(gameObject);
    }

    private void OnMessage(int fromDeviceID, JToken data)
    {
        if (data["action"] == null) return;
        if (data["action"].ToString().Equals("playerReady"))
        {
            playersReady.Add(fromDeviceID);
            if (playersReady.Count == playersConnected)
            {
                levelTransition.LoadLevel("MainGameScene", true);
            } 
            else
            {
                SetDeviceView(fromDeviceID, "waitingGame");
            }
        } 
    }

    private void OnConnect(int device_id)
    {
        if (device_id != 0)
        {
            playersConnected += 1;
            SetDeviceView(device_id, "menuNavigation");
        }
    }

    private void OnDisconnect(int device_id)
    {
        Debug.Log("Call disconect");
        playersConnected -= 1;
        if (playersReady.Contains(device_id))
        {
            playersReady.Remove(device_id);
        }
    }

    private void OnDestroy()
    {
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onMessage -= OnMessage;
            AirConsole.instance.onConnect -= OnConnect;
            AirConsole.instance.onDisconnect -= OnDisconnect;
        }
    }

    public void SetDeviceView(int deviceID, string viewName, bool isChallengerDevice = false)
    {
        var newView = new { view = viewName, isChallenger = isChallengerDevice };

        AirConsole.instance.Message(deviceID, newView);
    }

    public List<int> GetPlayersReady()
    {
        return playersReady;
    }
}
