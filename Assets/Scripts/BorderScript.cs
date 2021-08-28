using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderScript : MonoBehaviour
{
    private GameObject banner = null;
    public Transform bannerTransform;
    public Transform minigameBackground;
    
    public void SetBanner(GameObject newBanner) 
    {
        Destroy(banner);
        banner = Instantiate(newBanner, bannerTransform.position, Quaternion.identity);
    }
}
