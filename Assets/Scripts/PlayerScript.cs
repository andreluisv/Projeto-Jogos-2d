using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private List<Transform> waypoints;

    private List<GameObject> boardPlaces;

    private Transform currentTarget;

    private GameLogic gameLogic;

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

    private static readonly Color[] playerColors = {new Color(255,0,0,255), new Color(0,255,0,255), new Color(0,0,255,255)};
    private void Start()
    {
        gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
        waypoints = new List<Transform>();
        boardPlaces = new List<GameObject>();
        this.tag = "Player" + playerIndex;
        for (int i = 0; i < waypointsSize; i++)
        {
            GameObject waypoint = GameObject.Find("BoardWaypoints" + playerIndex + "/Waypoint" + i);
            GameObject place = GameObject.Find("BoardPlaces/Board" + i);
            waypoints.Add(waypoint.transform);
            boardPlaces.Add(place);
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
            waypointIndex %= waypointsSize;
            currentTarget = waypoints[(waypointIndex + 1) % waypointsSize];
            toMove -= 1;
            if (toMove == 0)
            {
                isMoving = false;
                BoardPositionLogic();
            }
        }
    }

    void BoardPositionLogic() 
    {
        if (boardPlaces[waypointIndex].tag != this.tag) 
        {
            if (boardPlaces[waypointIndex].tag == "Neutral") 
            {
                ChangeBoardPositionLeader();
                gameLogic.EndMove();   
            } 
            else 
            {
                gameLogic.Duel(this.tag[this.tag.Length-1]-'0', boardPlaces[waypointIndex].tag[boardPlaces[waypointIndex].tag.Length-1]-'0');
            }
        } 
        else 
        {
            gameLogic.EndMove();
        }
    }

    public void ChangeBoardPositionLeader()
    {
        boardPlaces[waypointIndex].GetComponent<SpriteRenderer>().color = playerColors[playerIndex];
        boardPlaces[waypointIndex].tag = this.tag;
    }
}
