using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    private GameObject[] waypoints;
    private GameObject[] boardPlaces;
    private Transform currentTarget;
    private GameLogic gameLogic;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource playerAudio;
    [SerializeField]
    private float moveSpeed = 5f;
    private int boardSize = -1;
    [HideInInspector]
    public bool gameOver = false;
    private int lifes = 3;
    public int waypointIndex = 0;
    private int laps = 0;
    private int toMove = 0;
    private int toMoveBack = 0;
    private int playerIndex;
    public bool isMoving = false;
    public bool isMovingBack = false;
    public bool isDamaged = false;
    public Image[] heartsPlayer;
    private int pointerHearts = 2;
    [SerializeField]
    private GameObject playerBanner;
    private bool isAttacking = false;
    private bool changingFlag = false;
    private float targetTime = 0.0f;
    private Characters playerCharacter;

    private void Start()
    {
        playerAudio = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        heartsPlayer = GameObject.Find("MainUI/Top_Screen_UI/Hearts_Position/Player"+playerIndex).GetComponentsInChildren<Image>(true);
        gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
        boardPlaces = gameLogic.GetBoardPlaces();
        waypoints = gameLogic.GetWaypoints(playerIndex);
        boardSize = boardPlaces.Length;
        this.tag = "Player" + playerIndex;
        currentTarget = waypoints[(waypointIndex + 1) % boardSize].transform;
        animator.SetBool("isHurt", false);
    }

    public void SetPlayerIndex(int playerIndex)
    {
        this.playerIndex = playerIndex;
        playerCharacter = (Characters) playerIndex;
    }

    public Characters GetCharacther()
    {
        return playerCharacter;
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
        if(isAttacking)
        {
            if(Time.time > targetTime)
            {
                isAttacking = false;
                animator.SetBool("isAttacking", false);
                changingFlag = true;
                ChangeBoardPositionLeader();
                targetTime = Time.time + 1.0f;
            }
        }
        if(changingFlag)
        {
            if(Time.time > targetTime)
            {
                changingFlag = false;
                gameLogic.CameraFocusOnPlayerToMove();
            }
        }
        if (isDamaged)
        {
            // Debug.Log("time: " + Time.time + " targert time: " + targetTime);
            if(Time.time > targetTime)
            {
                isDamaged = false;
                animator.SetBool("isHurt", false);
                gameLogic.CameraFocusOnPlayerToMove();
            
            }
        }
    }

    public void DiceRoll(int toMove)
    {
        this.toMove = toMove;
        this.toMoveBack = toMove;
        isMoving = true;
        playerAudio.Play();
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
                playerAudio.Stop();
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

    public void setTakeDamage()
    {
        this.loseLife();
        isDamaged = true;
        animator.SetBool("isHurt", true);
        targetTime = Time.time + 2.0f;
    }

    private void loseLife() 
    {
        if (lifes > 0) {
            lifes--;
            this.removeHeart();
        }
        if (lifes == 0)
        {
            setGameOver();
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
        Debug.Log(heartsPlayer.Length + " " + pointerHearts);
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
                StartCoroutine(TakeNeutral());
            } 
            else 
            {
                Transform background = boardPlaces[waypointIndex].GetComponent<BorderScript>().minigameBackground;
                gameLogic.Duel(this.tag[this.tag.Length-1]-'0', boardPlaces[waypointIndex].tag[boardPlaces[waypointIndex].tag.Length-1]-'0', background);
            }
        } 
        else 
        {
            StartCoroutine(TakeNeutral());
        }
    }

    IEnumerator TakeNeutral()
    {
        yield return new WaitForSeconds(1.25f);
        gameLogic.EndMove();
        gameLogic.CameraFocusOnPlayerToMove();
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
    public void setAttack() 
    {
        animator.SetBool("isAttacking", true);
        targetTime = Time.time + 2.0f;
        isAttacking = true;
    }
}
