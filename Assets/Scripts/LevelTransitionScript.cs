using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionScript : MonoBehaviour
{
    public Animator transitionAnimator; 

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadLevel(string levelName, bool useAnimation = false, float transitionTime = 0f, LoadSceneMode loadMode = LoadSceneMode.Single)
    {
        StartCoroutine(LoadLevelCoroutine(levelName, useAnimation, transitionTime, loadMode));
    }

    IEnumerator LoadLevelCoroutine(string levelName, bool useAnimation = false, float transitionTime = 0f, LoadSceneMode loadMode = LoadSceneMode.Single)
    {
        if (useAnimation)
        {
            FadeIn();
        }
        yield return new WaitForSeconds(transitionTime);
        if (useAnimation)
        {
            FadeOut();
        }
        SceneManager.LoadSceneAsync(levelName, loadMode);
    }

    public void FadeIn()
    {
        transitionAnimator.SetTrigger("Start");
    }
    
    public void FadeOut()
    {
        transitionAnimator.SetTrigger("End");
    }
}
