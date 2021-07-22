using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private List<Transform> waypoints;

    private Transform currentTarget;

    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private int waypointsSize = 6;

    [HideInInspector]
    public int waypointIndex = 0;

    private int laps = 0;

    private int toMove = 0;

    private int playerIndex;

    public bool isMoving = false;

    private void Start()
    {
        waypoints = new List<Transform>();
        for (int i = 0; i < waypointsSize; i++)
        {
            GameObject waypoint = GameObject.Find("BoardWaypoints" + playerIndex + "/Waypoint" + i);
            waypoints.Add(waypoint.transform);
        }
        transform.position = waypoints[waypointIndex].transform.position;
        currentTarget = waypoints[(waypointIndex + 1) % waypointsSize];

        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("SquareMeme");
    }

    public void SetPlayerIndex(int playerIndex)
    {
        this.playerIndex = playerIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            Move();
        }
    }

    public void DiceRoll(int toMove)
    {
        this.toMove = toMove;
        isMoving = true;
    }

    void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);
        if (transform.position == currentTarget.position)
        {
            waypointIndex += 1;
            currentTarget = waypoints[(waypointIndex + 1) % waypointsSize];
            toMove -= 1;
            if (toMove == 0)
            {
                isMoving = false;
            }
        }
    }
}
