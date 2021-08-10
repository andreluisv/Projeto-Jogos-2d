using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoKenPoUIScript : MonoBehaviour
{
    public GameObject loading;
    public GameObject leftSword;
    public GameObject rightSword;

    public void FlipLoadAnimation()
    {
        loading.SetActive(!loading.activeSelf);
        leftSword.SetActive(!leftSword.activeSelf);
        rightSword.SetActive(!loading.activeSelf);
    }

    public void EndGame()
    {
        loading.SetActive(false);
        leftSword.SetActive(false);
        rightSword.SetActive(false);
    }
}
