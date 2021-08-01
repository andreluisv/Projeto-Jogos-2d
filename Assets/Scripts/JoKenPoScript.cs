using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoKenPoScript : MonoBehaviour
{
    public JoKenPoUIScript gameUI;
    public GameLogic gameLogic;
    public GameObject[] leftChoices;
    public GameObject[] rightChoices;
    public Transform leftTarget, rightTarget, leftReset, rightReset;
    private int challenger, defender;
    private int leftID = 0, rightID = 0;
    [SerializeField]
    private float moveSpeed = 5f;
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
        gameUI.SetGameStatus("Draw");
        yield return new WaitForSeconds(5f);
        choiceCount = 0;
        leftChoices[leftID].transform.position = leftReset.position;
        rightChoices[rightID].transform.position = rightReset.position;
        gameUI.SetGameStatus("Waiting moves");
        gameLogic.SetDeviceView(challenger, "joKenPoOptions");
        gameLogic.SetDeviceView(defender, "joKenPoOptions");
    }

    IEnumerator EndGame()
    {
        if (winner == -1)
        {
            gameUI.SetGameStatus("Challenger Wins");
        }
        else if (winner == 1)
        {
            gameUI.SetGameStatus("Defender Wins");
        }
        gameLogic.SetWhoWins(winner);
        yield return new WaitForSeconds(5f);
        gameLogic.UnloadMiniGame("MiniGame0");
    }

    IEnumerator StartGame()
    {
        gameUI.SetGameStatus("Prepare for fight");
        yield return new WaitForSeconds(5f);
        isMoving = true;
    }

    public void ReceivePlayerChoice(int fromID, string choice)
    {
        if (fromID == challenger)
        {
            leftID = choiceID[choice];
        } 
        else if (fromID == defender)
        {
            rightID = choiceID[choice];
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
        Debug.Log(leftID + " " + rightID);
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
}
