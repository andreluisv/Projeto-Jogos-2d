using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JoKenPoUIScript : MonoBehaviour
{
    public GameObject loading;
    public GameObject rotatingTriangle;
    [SerializeField] private TextMeshProUGUI leftCard;
    [SerializeField] private TextMeshProUGUI rightCard;

    public void FlipLoadAnimation()
    {
        loading.SetActive(!loading.activeSelf);
        rotatingTriangle.SetActive(!rotatingTriangle.activeSelf);
    }

    public void EndGame()
    {
        loading.SetActive(false);
        rotatingTriangle.SetActive(false);
    }

    public void SetUIText(Characters left, Characters right)
    {
        leftCard.text = left.ToString();
        rightCard.text = right.ToString();
    }
}
