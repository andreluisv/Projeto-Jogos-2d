using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private GameObject[] waypoints;
    private GameObject[] boardPlaces;
    private Transform currentTarget;
    private GameLogic gameLogic;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private int boardSize = 6;
    [HideInInspector]
    public bool gameOver = false;
    public int lifes = 5;
    public int waypointIndex = 0;
    private int laps = 0;
    private int toMove = 0;
    private int toMoveBack = 0;
    private int playerIndex;
    public bool isMoving = false;
    public bool isMovingBack = false;
    public Image[] heartsPlayer;
    public int pointerHearts = 4;
    [SerializeField]
    private GameObject playerBanner;
    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        heartsPlayer = GameObject.Find("Canvas/Top_Screen_UI/Hearts_Position/Player"+playerIndex).GetComponentsInChildren<Image>(true);
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
            FixSpriteDirection();
        }
        if (isMovingBack) {
            MoveBack();
            FixSpriteDirectionBack();
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
                FixSpriteDirection();
                animator.SetBool("IsMoving", false);
                BoardPositionLogic();
            }
        }
    }

    public void setMoveBack()
    {
        this.loseLife();
        isMovingBack = true;
        animator.SetBool("IsMoving", true);
    }

    private void loseLife() 
    {
        if (lifes > 0) {
            lifes--;
            this.removeHeart();
        } else {
            this.setGameOver();
        }
    }

    public int getLifes() {
        return lifes;
    }

    public bool getGameOver() {
        return gameOver;
    }

    private void setGameOver() {
        gameOver = true;
    }

    private void removeHeart() {
        heartsPlayer[pointerHearts].sprite = Resources.Load<Sprite>("Sprites/Hearts/HeartsFrame3");
        pointerHearts--;
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
                gameLogic.CameraFocusOnPlayerToMove();
                isMovingBack = false;
                FixSpriteDirection();
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
                gameLogic.CameraFocusOnPlayerToMove();
            } 
            else 
            {
                gameLogic.Duel(this.tag[this.tag.Length-1]-'0', boardPlaces[waypointIndex].tag[boardPlaces[waypointIndex].tag.Length-1]-'0');
            }
        } 
        else 
        {
            gameLogic.EndMove();
            gameLogic.CameraFocusOnPlayerToMove();
        }
    }

    public void ChangeBoardPositionLeader()
    {
        boardPlaces[waypointIndex].GetComponent<BorderScript>().SetBanner(playerBanner);
        boardPlaces[waypointIndex].tag = this.tag;
    }

    private void FixSpriteDirection()
    {
        Vector3 dir = currentTarget.position - transform.position;
        if (Mathf.Sign(dir.x) == -1)
        {
            spriteRenderer.flipX = true;
        } 
        else if (Mathf.Sign(dir.x) == 1)
        {
            spriteRenderer.flipX = false;
        }
    }
    private void FixSpriteDirectionBack()
    {
        Transform backTarget = waypoints[(waypointIndex - 1 + boardSize) % boardSize].transform;
        Vector3 dir = backTarget.position - transform.position;
        if (Mathf.Sign(dir.x) == -1)
        {
            spriteRenderer.flipX = true;
        } 
        else if (Mathf.Sign(dir.x) == 1)
        {
            spriteRenderer.flipX = false;
        }
    }
}
