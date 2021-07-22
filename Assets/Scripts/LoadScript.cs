using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameLogic gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
        if (gameLogic == null) return;
        gameLogic.dice = GameObject.Find("Dice").GetComponent<DiceScript>();
        for (int i = 0; i < gameLogic.playersID.Count; i++)
        {
            GameObject newPlayer = new GameObject("Player" + i);
            newPlayer.AddComponent<SpriteRenderer>();
            newPlayer.AddComponent<PlayerScript>();
            newPlayer.GetComponent<PlayerScript>().SetPlayerIndex(i);
            gameLogic.players.Add(newPlayer);
        }

    }
}
