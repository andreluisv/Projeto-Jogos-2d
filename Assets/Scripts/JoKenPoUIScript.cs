using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoKenPoUIScript : MonoBehaviour
{
    public GameObject loading;
    public GameObject rotatingTriangle;

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
}
