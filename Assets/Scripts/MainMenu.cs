using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuFirstButton, OptionsMenuFirstButton;
    public void PlayGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game");
        Application.Quit();
    }
    public void ClearSelection()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void SetMainMenuSelection()
    {
        ClearSelection();
        EventSystem.current.SetSelectedGameObject(MainMenuFirstButton);
    }
    public void SetOptionsMenuSelection()
    {
        ClearSelection();
        EventSystem.current.SetSelectedGameObject(OptionsMenuFirstButton);
    }
}
