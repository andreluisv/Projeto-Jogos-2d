using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScript : MonoBehaviour
{

    // Start is called before the first frame update
    public GameObject joKenPoUI;
    private Sprite[] sprites;
    private string spritePath = "Sprites";
    void Start()
    {
        GameLogic gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
        if (gameLogic == null) return;
        gameLogic.dice = GameObject.Find("Dice").GetComponent<DiceScript>();
        gameLogic.cameraObj = GameObject.Find("Main Camera");
        gameLogic.miniGames.Add(joKenPoUI);


        sprites = Resources.LoadAll<Sprite>(spritePath);

        /*
        foreach(var e in sprites) {
            Debug.Log(e.name);
        }
        */
        for (int i = 0; i < gameLogic.playersID.Count; i++)
        {
            GameObject newPlayer = new GameObject("Player" + i);
            
            newPlayer.AddComponent<SpriteRenderer>();
            newPlayer.GetComponent<SpriteRenderer>().sprite = sprites[i];

            newPlayer.AddComponent<PlayerScript>();
            newPlayer.GetComponent<PlayerScript>().SetPlayerIndex(i);
            gameLogic.players.Add(newPlayer);
        }
        // SceneManager.LoadScene("MiniGame0", LoadSceneMode.Additive);
        //SceneManager.UnloadSceneAsync("MiniGame0");
    }
}
