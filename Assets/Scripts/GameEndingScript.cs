using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameEndingScript : MonoBehaviour
{
    public TextMeshProUGUI cardText;
    public Image cardImage;
    public Sprite hawySprite;
    public Sprite fionaSprite;

    void Start()
    {
        GameLogic gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
        Characters winner = gameLogic.GetGameWinner();
        if (winner == Characters.Hawy)
        {
            cardText.text = "Hawy has survived the trials";
            cardImage.sprite = hawySprite;   
        }
        else if (winner == Characters.Fiona)
        {
            cardText.text = "Fiona has survived the trials";
            cardImage.sprite = fionaSprite;
        }
        StartCoroutine(EndingCouroutine());
    }

    IEnumerator EndingCouroutine()
    {
        yield return new WaitForSeconds(10f);
        Application.Quit();
    }
}
