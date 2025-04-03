using System;
using UnityEngine;

public class PongEnemyAIPaddle : PongPaddle
{
    //Enemy speed
    private float enemyPaddleMoveSpeedMax = 12f;
    private float enemyPaddleMoveSpeedMin = 0f;
    private float currentEnemyPaddleMoveSpeed = 0f;

    //local vars for tracking positions
    private float paddlePosY = 0f;
    private Vector2 nextBallPos;
    private float targetPosY = 0f;
    private float yPosAbsDiff = 0f;
    private float minReqYPosDiffToMove = 0.25f;
    private float nextStep = 0f;

    //Error rate so AI isn't perfect and can be beat
    private float minErrorRange = 0.25f;
    private float minErrorRangeDefault = 0.25f;
    private float maxErrorRange = 10f;
    private float currMaxErrorRange = 0f;
    private float currentErrorMargin = 0f;
    private bool needNewErrorMargin = false;

    //middle of paddle shots limiter
    private int maxNumCenterShotsAllowedBeforeAdjusting = 2;
    
    void Update()
    {
        if (isAlive) //if the game is over, we don't want the paddle moving
        {
            PlayPong();
        }
    }

    //Enemy AI for playing Pong
    //general approach is to predict the position of the ball after next 1-2 bounces
    //then deliberately generate errors as ball speed increases so AI isn't perfect
    private void PlayPong()
    {
        //once per paddle hit, generate an error margin so AI is not perfect
        if (!needNewErrorMargin && freshlyHitBall == true)
        {
            freshlyHitBall = false;
            minErrorRange = minErrorRangeDefault;
            needNewErrorMargin = true;
        }

        if (needNewErrorMargin)
        {
            needNewErrorMargin = false;

            if (Mathf.Approximately(0f, ball.GetBallVelocity().y))
            {
                currentErrorMargin = 0f; //no error if ball is flat
            }
            else
            {
                currentErrorMargin = GetNewErrorValue();
            }
            Debug.Log("new error margin = " + currentErrorMargin.ToString());
        }

        //update position vars
        paddlePosY = transform.position.y;
        nextBallPos = ball.GetNextBallHitPosition();
        targetPosY = nextBallPos.y + currentErrorMargin;

        //it's boring if AI keeps doing center-of-paddle shots so don't do too many in a row
        if (numCenterPaddleShotsInARow >= maxNumCenterShotsAllowedBeforeAdjusting)
        {
            targetPosY = nextBallPos.y + 0.75f;
        }
        else
        {
            targetPosY = nextBallPos.y + currentErrorMargin;
        }

        //how far away from the ball is this paddle?
        yPosAbsDiff = Mathf.Abs(targetPosY - paddlePosY);

        //only move to meet ball if we are a certain distance away, otherwise stay put
        if (yPosAbsDiff > minReqYPosDiffToMove)
        {
            //scale our current move speed based on distance away from target
            currentEnemyPaddleMoveSpeed = ScaleFloatToNewInputRange(yPosAbsDiff, minPaddlePosY, maxPaddlePosY, 
                                                                    enemyPaddleMoveSpeedMin, enemyPaddleMoveSpeedMax);

            if (paddlePosY < targetPosY)
            {
                //below the target so need to move up to meet it

                nextStep = currentEnemyPaddleMoveSpeed * Time.deltaTime;

                if (InBounds(transform.position.y + nextStep))
                {
                    transform.Translate(0f, nextStep, 0f);
                }                    
            }
            else if (paddlePosY > targetPosY)
            {
                //above the target so need to move down to meet it

                nextStep = -1f * currentEnemyPaddleMoveSpeed * Time.deltaTime;

                if (InBounds(transform.position.y + nextStep))
                {
                    transform.Translate(0f, nextStep, 0f);
                }
            }
        }
    }

    //Helper function to scale speed based on how far away from target position the paddle is
    private float ScaleFloatToNewInputRange(float oldValue, float oldMin, float oldMax, float newMin, float newMax)
    {
        float newRange = newMax - newMin;
        float newValue = Mathf.InverseLerp(oldMin, oldMax, oldValue) * newRange;

        if (newValue >= newMax)
        {
            return newMax;
        }
        else if (newValue <= newMin)
        {
            return newMin;
        }
        else
        {
            return newValue;
        }
    }

    private float GetNewErrorValue()
    {
        //AI errors increase as ball speed on Y-axis increases
        minErrorRange = ScaleFloatToNewInputRange(ball.GetBallVelocity().y, 
                                                    ball.GetBallMinVelocity(), ball.GetBallMaxVelocity(), 
                                                    minErrorRange, maxErrorRange);

        //this encourages player to bounce the ball using edges of paddle (more fun) 
        currMaxErrorRange = ScaleFloatToNewInputRange(ball.GetBallVelocity().y, 
                                                    ball.GetBallMinVelocity(), ball.GetBallMaxVelocity(), 
                                                    minErrorRange, maxErrorRange);

        float newError = UnityEngine.Random.Range(-1f * currMaxErrorRange, currMaxErrorRange);

        //clamp newError to our desired range
        if (newError >= maxErrorRange)
        {
            newError = maxErrorRange;
        }
        else if (newError <= minErrorRange)
        {
            newError = minErrorRange;
        }
        
        //randomly pick pos or negative error value
        bool positive = UnityEngine.Random.value >= 0.5f;
        if (positive)
        {
            return newError;
        }
        else
        {
            return newError * -1f;
        }
    }

    //Helper function to determine if proposed movement is in bounds of game
    private bool InBounds(float newY)
    {
        //Do we have room to move? Check position against max and min movement range
        return newY > minPaddlePosY && newY < maxPaddlePosY;
    }
}
