using System;
using UnityEngine;

public class PongPaddle : MonoBehaviour
{
    //game controller ref
    protected PongGameManager gameManager;

    //the paddle is "alive" as long as neither player has won the game
    protected bool isAlive = true;

    //Pong ball
    protected PongBall ball;
    protected Vector2 localBallVelocity;
    protected float velocityIncFactorOnPaddleHitCenter = 1.35f;
    protected float velocityIncFactorOnPaddleHitOnEdge = 1.5f;

    //Court boundaries to prevent paddle going off-screen
    protected float maxPaddlePosY = 5f;
    protected float minPaddlePosY = -5f;

    //SFX
    protected AudioSource paddleHitsBallSFX;

    //track where ball hit paddle
    private float relativeXOfPaddleToBall; 
    private float relativeYOfBallToPaddle;

    //track shots
    protected int numCenterPaddleShotsInARow = 0;
    protected int numShotsInARow = 0;
    protected bool freshlyHitBall = false;

    void Start()
    {
        //refs in the scene
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PongGameManager>();
        ball = gameManager.ball;

        //refs to local components
        paddleHitsBallSFX = GetComponent<AudioSource>();
    }

    public void SetIsAlive(bool alive)
    {
        isAlive = alive;
    }

    public void ResetBallServeBool()
    {
        freshlyHitBall = true;
        numShotsInARow = 0;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Did we hit the ball? (other possibility is hitting the wall PongCourtBorder, which should do nothing)
        if (collision.gameObject.CompareTag("Ball"))
        {
            if (ball.CanBallBeHitByThisGameObject(gameObject)) //make sure same paddle doesn't hit ball twice in a row
            {
                //figure out if ball hit front of paddle? if not, we do nothing and this paddle "missed" the ball
                relativeXOfPaddleToBall = Math.Abs(transform.position.x) - Math.Abs(collision.gameObject.transform.position.x);

                if (relativeXOfPaddleToBall >= 0)
                {
                    //we hit front of the paddle
                    //Debug.Log("hit front of paddle");

                    if (paddleHitsBallSFX)
                    {
                        paddleHitsBallSFX.Play();
                    }

                    numShotsInARow++;

                    //paddle is divided into three parts
                    //paddle center should send the ball straight forward (inc. X velocity, but zero out Y velocity)
                    //paddle top/bottom should send the ball back with bounce (inc. X velocity and inc. Y velocity)
                    
                    //get current ball velocity and invert it for the return trip
                    localBallVelocity = ball.GetBallVelocity();

                    //figure out where on the paddle (relatively) that the ball struck the paddle (top, center, or bottom?)
                    relativeYOfBallToPaddle = collision.gameObject.transform.position.y - transform.position.y;

                    if (relativeYOfBallToPaddle <= 0.25f && relativeYOfBallToPaddle >= -0.25f)
                    {
                        //paddle hit ball with the center of the paddle => fires straight back
                        ball.SetNewBallVelocity(localBallVelocity.x * velocityIncFactorOnPaddleHitCenter, 0f);
                        //Debug.Log("hit center " + relativeYOfBallToPaddle.ToString());
                        numCenterPaddleShotsInARow++;
                    }
                    else if (relativeYOfBallToPaddle < -0.25f)
                    {
                        //paddle hit ball with bottom of paddle => bounces down to bottom of screen with inc. speeds
                        if (localBallVelocity.y == 0)
                        {
                            localBallVelocity.y = 5f;
                        }
                        ball.SetNewBallVelocity(-1f * localBallVelocity.x * velocityIncFactorOnPaddleHitOnEdge, 
                                                    -1f * Math.Abs(localBallVelocity.y * velocityIncFactorOnPaddleHitOnEdge));
                        numCenterPaddleShotsInARow = 0;
                        //Debug.Log("hit bottom " + relativeYOfBallToPaddle.ToString());
                    }
                    else if (relativeYOfBallToPaddle > 0.25f)
                    {
                        //paddle hit ball with top of paddle => bounces up to top of screen with inc. speeds
                        if (localBallVelocity.y == 0)
                        {
                            localBallVelocity.y = 5f;
                        }
                        ball.SetNewBallVelocity(-1f * localBallVelocity.x * velocityIncFactorOnPaddleHitOnEdge, 
                                                    Math.Abs(localBallVelocity.y * velocityIncFactorOnPaddleHitOnEdge));
                        numCenterPaddleShotsInARow = 0;
                        //Debug.Log("hit top " + relativeYOfBallToPaddle.ToString());
                    }
                }
            }
        }
    }
}
