using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PongGameManager : MonoBehaviour
{
    //Game Manager holds refs to everything relevant to the game
    //if other scripts need a reference, all they do is find GameManager
    //and then ask for that reference

    //the ball ref (for resetting between rounds and games)
    public PongBall ball;

    //paddles
    public PongPaddle player1Paddle;
    public PongPaddle player2Paddle;

    //Scoring
    public int maxScore = 11;
    private int scorePlayerOne;
    private int scorePlayerTwo;

    //Game UI elements
    public TextMeshProUGUI scoreOne;
    public TextMeshProUGUI scoreTwo;
    public TextMeshProUGUI player1Wins;
    public TextMeshProUGUI player2Wins;
    public GameObject btnRestartGame;

    //scoring particles
    public ParticleSystem particlesForRedScored;
    public ParticleSystem particlesForBlueScored;

    //timer before serving ball
    private float timer;
    private float serveTimerThreshold = 0.5f;
    private bool waitingToServeBall = true;

    void Start()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        //Hide the restart button
        if (btnRestartGame != null)
        {
            btnRestartGame.SetActive(false);
        }

        //Reset all the game UI back to default
        if (player1Wins != null)
        {
            player1Wins.enabled = false;
        }
        if (player2Wins != null)
        {
            player2Wins.enabled = false;
        }
        
        scorePlayerOne = 0;
        scorePlayerTwo = 0;
        scoreOne.text = scorePlayerOne.ToString();
        scoreTwo.text = scorePlayerTwo.ToString();

        //enable paddle movement
        player1Paddle.SetIsAlive(true);
        player2Paddle.SetIsAlive(true);

        //Start a new round of Pong
        StartNewRound();
    }

    void Update()
    {
        //this is to prevent ball from instantly serving after each point
        //instead brief delay before new ball is served
        if (waitingToServeBall)
        {
            timer += Time.deltaTime;

            if (timer > serveTimerThreshold)
            {
                waitingToServeBall = false;
                timer = 0f;
                ball.ServeBall();
            }
        }
    }

    public void StartNewRound()
    {
        timer = 0f;
        waitingToServeBall = true;
    }

    public void IncScorePlayerOne()
    {
        scorePlayerOne++;
        scoreOne.text = scorePlayerOne.ToString();
        particlesForBlueScored.Play();

        if (scorePlayerOne >= maxScore)
        {
            //game over, player one wins
            player1Wins.enabled = true;
            GameOverFlow();
        }
        else
        {
            StartNewRound();
        }
    }

    public void IncScorePlayerTwo()
    {
        scorePlayerTwo++;
        scoreTwo.text = scorePlayerTwo.ToString();
        particlesForRedScored.Play();
        
        if (scorePlayerTwo >= maxScore)
        {
            //game over, player two wins
            player2Wins.enabled = true;
            GameOverFlow();
        }
        else
        {
            StartNewRound();
        }
    }

    private void GameOverFlow()
    {
        //show restart button
        btnRestartGame.SetActive(true);

        //stop the ball from moving
        ball.ResetAndFreezeBall();

        //disable paddle movement
        player1Paddle.SetIsAlive(false);
        player2Paddle.SetIsAlive(false);
    }
}
