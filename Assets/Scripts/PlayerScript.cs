using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private GameObject[] waypoints;
    private GameObject[] boardPlaces;
    private Transform currentTarget;
    private GameLogic gameLogic;
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private int boardSize = 6;
    [HideInInspector]
    public int waypointIndex = 0;
    private int laps = 0;
    private int toMove = 0;
    private int toMoveBack = 0;
    private int playerIndex;
    public bool isMoving = false;
    public bool isMovingBack = false;

    private static readonly Color[] playerColors = {new Color(255,0,0,255), new Color(0,255,0,255), new Color(0,0,255,255)};
    private void Start()
    {
        gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
        boardPlaces = gameLogic.getBoardPlaces(); // new List<GameObject>();
        waypoints = gameLogic.getWaypoints(playerIndex);
        this.tag = "Player" + playerIndex;
        // for (int i = 0; i < boardSize; i++)
        // {
        //     GameObject waypoint = GameObject.Find("BoardWaypoints" + playerIndex + "/Waypoint" + i);
        //     GameObject place = GameObject.Find("BoardPlaces/Board" + i);
        //     waypoints.Add(waypoint.transform);
        //     boardPlaces.Add(place);
        // }
        transform.position = waypoints[waypointIndex].transform.position;
        currentTarget = waypoints[(waypointIndex + 1) % boardSize].transform;
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
        if (isMovingBack) {
            MoveBack();
        }
    }

    public void DiceRoll(int toMove)
    {
        this.toMove = toMove;
        this.toMoveBack = toMove;
        isMoving = true;
    
    }

    void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);
        if (transform.position == currentTarget.position)
        {
            waypointIndex += 1;
            waypointIndex %= boardSize;
            currentTarget = waypoints[(waypointIndex + 1) % boardSize].transform;
            toMove -= 1;
            if (toMove == 0)
            {
                isMoving = false;
                BoardPositionLogic();
            }
        }
    }

    public void setMoveBack()
    {
        isMovingBack = true;
    }
    void MoveBack()
    {
        Transform backTarget = waypoints[(waypointIndex - 1 + boardSize) % boardSize].transform;
        currentTarget = waypoints[(waypointIndex + 1) % boardSize].transform;
        transform.position = Vector2.MoveTowards(transform.position, backTarget.position, moveSpeed * Time.deltaTime);
        if (transform.position == backTarget.position)
        {
            waypointIndex = waypointIndex - 1 + boardSize;
            waypointIndex %= boardSize;
            backTarget = waypoints[(waypointIndex - 1 + boardSize) % boardSize].transform;
            currentTarget = waypoints[(waypointIndex + 1) % boardSize].transform;
            toMoveBack -= 1;
            
            if (toMoveBack == 0)
            {
                isMovingBack = false;
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
