using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject currentTarget;
    private bool follow = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (follow)
        {
            transform.position = new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y, transform.position.z);
        }
    }

    void setFollow(bool newFollow)
    {
        follow = newFollow;
    }

    void setCurrentTarget(GameObject newCurrentTarget)
    {
        currentTarget = newCurrentTarget;
    }
}
