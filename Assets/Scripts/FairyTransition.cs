using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FairyTransition : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject victoryHawy;
    [SerializeField] private GameObject victoryFiona;
    [SerializeField] private GameObject defeatHawy;
    [SerializeField] private GameObject defeatFiona;
    void Start()
    {
        int curWinner = PlayerPrefs.GetInt("curWinner");
        int curCharacter = PlayerPrefs.GetInt("character");
        if (curCharacter == 0 && curWinner == -1)
        {
            victoryHawy.SetActive(true);
        }
        else if (curCharacter == 1 && curWinner == -1) 
        {
            victoryFiona.SetActive(true);
        }
        else if (curCharacter == 0 && curWinner == 1) 
        {
            defeatHawy.SetActive(true);   
        }
        else if (curCharacter == 1 && curWinner == 1) 
        {
            defeatFiona.SetActive(true);
        }
        StartCoroutine(BeginAnimate());
    }

    IEnumerator BeginAnimate()
    {
        yield return new WaitForSeconds(2f);
        animator.SetBool("Eyes", true);
    }
}
