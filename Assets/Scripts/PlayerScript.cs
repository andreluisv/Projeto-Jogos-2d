using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private GameObject[] waypoints;
    private GameObject[] boardPlaces;
    private Transform currentTarget;
    private GameLogic gameLogic;
    private Animator animator;
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
    [SerializeField]
    private Color playerColor;
    private void Start()
    {
        animator = GetComponent<Animator>();
        gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
        boardPlaces = gameLogic.GetBoardPlaces();
        waypoints = gameLogic.GetWaypoints(playerIndex);
        this.tag = "Player" + playerIndex;
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
        animator.SetBool("IsMoving", true);
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
                animator.SetBool("IsMoving", false);
                BoardPositionLogic();
            }
        }
    }

    public void setMoveBack()
    {
        isMovingBack = true;
        animator.SetBool("IsMoving", true);
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
                animator.SetBool("IsMoving", false);
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
        boardPlaces[waypointIndex].GetComponent<SpriteRenderer>().color = playerColor;
        boardPlaces[waypointIndex].tag = this.tag;
    }
}
