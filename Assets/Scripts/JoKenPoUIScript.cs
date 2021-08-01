using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoKenPoUIScript : MonoBehaviour
{
    public Text status;
    public void SetGameStatus(string newStatus)
    {
        status.text = newStatus;
    }
}
