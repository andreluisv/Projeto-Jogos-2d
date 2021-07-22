using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceScript : MonoBehaviour
{
    private Sprite[] diceSides;

    private SpriteRenderer rend;

    private int playerTurn = 0;

    private bool coroutineAllowed = true;

    private GameLogic gameLogic;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        diceSides = Resources.LoadAll<Sprite>("Dices/");
        rend.sprite = diceSides[5];
        gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
    }

    private void OnMouseDown()
    {
        if (coroutineAllowed)
        {
            StartCoroutine("RollDice");
        }
    }

    private IEnumerator RollDice()
    {
        coroutineAllowed = false;
        int randomDiceSide = 0;
        for (int i = 0; i < 20; i++)
        {
            randomDiceSide = Random.Range(0, 6);
            rend.sprite = diceSides[randomDiceSide];
            yield return new WaitForSeconds(0.05f);
        }
        // fazer mais alguma coisa
        coroutineAllowed = true;
        gameLogic.MovePlayer(randomDiceSide + 1);
    }
}
