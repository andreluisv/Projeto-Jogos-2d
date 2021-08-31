using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class JoKenPoScript : MonoBehaviour, IMiniGameScript
{
    public JoKenPoUIScript gameUI;
    public GameLogic gameLogic;
    public GameObject[] leftChoices;
    public GameObject[] rightChoices;
    public Transform leftTarget, rightTarget, leftReset, rightReset;
    private int challenger, defender;
    private int leftID = 0, rightID = 0;
    [SerializeField]
    private float moveSpeed = 8f;
    private int choiceCount = 0;
    private int winner = 0;
    private bool isMoving = false;
    private static readonly Dictionary<string,int> choiceID = new Dictionary<string, int>()
    {
        { "Rock", 0 },
        { "Papper", 1 },
        { "Scissors", 2 }
    };
    // Start is called before the first frame update
    void Start()
    {
        gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            Vector3 leftPos = leftChoices[leftID].transform.position, rightPos = rightChoices[rightID].transform.position;
            if (leftPos == leftTarget.position && rightPos == rightTarget.position)
            {
                isMoving = false;
                if (winner == 0)
                {
                    StartCoroutine(ResetGame());
                }
                else 
                {
                    StartCoroutine(EndGame());
                }
            }
            else 
            {
                leftChoices[leftID].transform.position = Vector2.MoveTowards(leftPos, leftTarget.position, moveSpeed * Time.deltaTime);
                rightChoices[rightID].transform.position = Vector2.MoveTowards(rightPos, rightTarget.position, moveSpeed * Time.deltaTime);
            }
        }
    }

    IEnumerator ResetGame() 
    {
        // gameUI.SetGameStatus("Draw");
        yield return new WaitForSeconds(2.5f);
        choiceCount = 0;
        leftChoices[leftID].transform.position = leftReset.position;
        rightChoices[rightID].transform.position = rightReset.position;
        gameUI.FlipLoadAnimation();
        gameLogic.SetDeviceView(challenger, "joKenPoOptions", true);
        gameLogic.SetDeviceView(defender, "joKenPoOptions", false);
    }

    IEnumerator EndGame()
    {
        if (winner == -1)
        {
            // do something
        }
        else if (winner == 1)
        {
            // do something
        }
        gameUI.EndGame();
        gameLogic.SetWhoWins(winner);
        yield return new WaitForSeconds(2.5f);
        gameLogic.UnloadMiniGame("MiniGame0");
    }

    IEnumerator StartGame()
    {
        gameUI.FlipLoadAnimation();
        yield return new WaitForSeconds(2.5f);
        isMoving = true;
    }

    public void ReceivePlayerData(int fromID, JToken data)
    {
        if (fromID == challenger)
        {
            leftID = choiceID[data["JoKenPoMove"].ToString()];
        } 
        else if (fromID == defender)
        {
            rightID = choiceID[data["JoKenPoMove"].ToString()];
        }
        choiceCount += 1;
        if (choiceCount == 2)
        {
            SetWinner();
            gameLogic.SetMiniGamePlayersView("doingCombat");
            StartCoroutine(StartGame());
        }
        else 
        {
            gameLogic.SetDeviceView(fromID, "waitForCombat");
        }
    }

    public void SetUIActive()
    {
        gameUI.gameObject.SetActive(true);
    }

    public void SetPlayers(int curChallenger, int curDefender)
    {
        challenger = curChallenger;
        defender = curDefender;
    }

    public void SetWinner() 
    {
        int aux = leftID - rightID;
        if (aux == 0) 
        {
            winner = 0;
        } 
        else if (aux == -1) 
        {
            winner = 1;
        } 
        else if (aux == 1) 
        {
            winner = -1;
        }
        else if (aux == -2) 
        {
            winner = -1;
        } 
        else if (aux == 2) 
        {
            winner = 1;
        }
    }

    public void SetUIText(Characters left, Characters right)
    {
        gameUI.SetUIText(left,right);
    }
}
