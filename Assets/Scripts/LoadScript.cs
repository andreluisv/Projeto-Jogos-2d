using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScript : MonoBehaviour
{

    // Start is called before the first frame update
    public GameObject joKenPoUI;
    private Sprite[] sprites;
    private Sprite sprite;
    private string spriteNames = "Assets/Resources";
    private SpriteRenderer spriteR;
    void Start()
    {
        GameLogic gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
        if (gameLogic == null) return;
        gameLogic.dice = GameObject.Find("Dice").GetComponent<DiceScript>();
        gameLogic.cameraObj = GameObject.Find("Main Camera");
        gameLogic.miniGames.Add(joKenPoUI);


        spriteR = gameObject.GetComponent<SpriteRenderer>();
        sprite = Resources.Load<Sprite>("Assets/Resources/Capsule");

        GameObject delicia = new GameObject("Delicia");
        delicia.AddComponent<SpriteRenderer>();
        

        for (int i = 0; i < gameLogic.playersID.Count; i++)
        {
            GameObject newPlayer = new GameObject("Player" + i);
            

            //SpriteRenderer spriteR = new SpriteRenderer();
            //spriteR.sprite = Resources.Load<Sprite>("Assets/Resources/Capsule");
            newPlayer.AddComponent<SpriteRenderer>();
            newPlayer.GetComponent<SpriteRenderer>().sprite = sprite;

            newPlayer.AddComponent<PlayerScript>();
            newPlayer.GetComponent<PlayerScript>().SetPlayerIndex(i);
            gameLogic.players.Add(newPlayer);
        }
        // SceneManager.LoadScene("MiniGame0", LoadSceneMode.Additive);
        //SceneManager.UnloadSceneAsync("MiniGame0");
    }
}
