using System;
using UnityEngine;

public class PongBall : MonoBehaviour
{
    //game controller ref
    protected PongGameManager gameManager;

    //Paddles
    private PongPaddle player1Paddle;
    private PongPaddle player2Paddle;

    //Local components on our Pong Ball
    private Rigidbody2D myRigidbody;
    private TrailRenderer ballTrail;

    //Ball speed
    private float startingVelocity = 5f;
    private float currVelocityX;
    private float currVelocityY;
    private float maximumVelocity = 14f;

    //Predicting ball movements
    Vector2 directionOfBall;
    Vector2 directionOfBallAfterNextBounce;
    RaycastHit2D nextHit;
    RaycastHit2D nextNextHit;

    //Serve direction counters
    private int numServesRight = 0;
    private int numServesLeft = 0;
    bool startOutGoingRight;
    bool startOutGoingUp;

    private GameObject lastObjectHit;

    void Start()
    {
        //refs in the scene
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PongGameManager>();
        player1Paddle = gameManager.player1Paddle;
        player2Paddle = gameManager.player2Paddle;

        //refs to local components
        myRigidbody = GetComponent<Rigidbody2D>();
        ballTrail = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        PredictNextBallHitPosition();
    }

    private void PredictNextBallHitPosition()
    {
        directionOfBall = new Vector2(myRigidbody.linearVelocity.x, myRigidbody.linearVelocity.y);
        
        nextHit = Physics2D.Raycast(transform.position, directionOfBall, 100f);
        if (nextHit)
        {
            //Debug.Log("ball headed toward something located at = " + nextHit.point);
            //Debug.DrawRay(transform.position, directionOfBall * nextHit.distance, Color.green);

            directionOfBallAfterNextBounce = new Vector2(directionOfBall.x, -1f * directionOfBall.y);
            
            nextNextHit = Physics2D.Raycast(nextHit.point, directionOfBallAfterNextBounce, 100f);

            /*if (nextNextHit)
            {
                Debug.DrawRay(nextHit.point, directionOfBallAfterNextBounce * nextNextHit.distance, Color.yellow);
            }*/
        }
    }

    public Vector2 GetNextBallHitPosition()
    {
        if (nextNextHit)
        {
            return nextNextHit.point;
        }
        else
        {
            return nextHit.point;
        }
    }

    public void ResetAndFreezeBall()
    {
        //turn off trail renderer and clear it
        ballTrail.emitting = false;
        ballTrail.Clear();
        ballTrail.enabled = false;

        //stop ball movement
        myRigidbody.linearVelocity = new Vector2(0,0);
        
        //reset position to center of court
        transform.position = new Vector3(0,0,0);

        //reset vars on paddles for tracking shots per serve (supports AI)
        lastObjectHit = null;
        player1Paddle.ResetBallServeBool();
        player2Paddle.ResetBallServeBool();
    }
    
    public void ServeBall()
    {
        ResetAndFreezeBall();

        if (numServesLeft == numServesRight)
        {
            //randomly serve the ball left or right, plus up or down
            startOutGoingRight = UnityEngine.Random.value >= 0.5f;
        }
        else if (numServesLeft > numServesRight)
        {
            startOutGoingRight = true;
        }
        else
        {
            startOutGoingRight = false;
        }
        

        if (startOutGoingRight)
        {
            currVelocityX = 1f;
            numServesRight++;
        }
        else
        {
            currVelocityX = -1f;
            numServesLeft++;
        }
        
        startOutGoingUp = UnityEngine.Random.value >= 0.5f;
        if (startOutGoingUp)
        {
            currVelocityY = 1f;
        }
        else
        {
            currVelocityY = -1f;
        }

        //apply starting speeds to the ball
        SetNewBallVelocity(currVelocityX * startingVelocity, currVelocityY * startingVelocity);

        //turn the trail back on
        ballTrail.enabled = true;
        ballTrail.emitting = true;
    }

    public Vector2 GetBallVelocity()
    {
        return new Vector2(currVelocityX, currVelocityY);
    }

    public void SetNewBallVelocity(float newVelocityX, float newVelocityY)
    {
        if (newVelocityX <= maximumVelocity)
        {
            currVelocityX = newVelocityX;
        }
        
        if (newVelocityY <= maximumVelocity)
        {
            currVelocityY = newVelocityY;
        }
        
        myRigidbody.linearVelocity = new Vector2(currVelocityX, currVelocityY);

        //Debug.Log("new ball speed = " + currVelocityX.ToString() + ", " + currVelocityY.ToString());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("ball hit by " + collision.gameObject.tag);

        //we just bounced off something, so update velocity vars
        currVelocityX = myRigidbody.linearVelocity.x;
        currVelocityY = myRigidbody.linearVelocity.y;

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            //reset vars on paddles for tracking shots per serve (supports AI)
            player1Paddle.ResetBallServeBool();
            player2Paddle.ResetBallServeBool();
        }
    }

    public bool CanBallBeHitByThisGameObject(GameObject newGO)
    {
        if (newGO.Equals(lastObjectHit))
        {
            return false;
        }
        else
        {
            lastObjectHit = newGO;
            return true;
        }
    }

    public float GetBallMinVelocity()
    {
        return startingVelocity;
    }

    public float GetBallMaxVelocity()
    {
        return maximumVelocity;
    }
}
