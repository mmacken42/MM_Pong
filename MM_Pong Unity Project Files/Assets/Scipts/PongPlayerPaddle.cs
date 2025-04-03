using UnityEngine;

public class PongPlayerPaddle : PongPaddle
{
    //player paddle top speed
    private float playerPaddleMoveSpeed = 12f;

    void Update()
    {
        if (isAlive) //if the game is over, we don't want the paddle moving
        {
            HandlePlayerInput();
        }
    }

    private void HandlePlayerInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (transform.position.y < maxPaddlePosY)
            {
                //move up
                transform.Translate(0f, playerPaddleMoveSpeed * Time.deltaTime, 0f);
            }
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (transform.position.y > minPaddlePosY)
            {
                //move down
                transform.Translate(0f, -1f * playerPaddleMoveSpeed * Time.deltaTime, 0f);
            }
        }
    }
}
