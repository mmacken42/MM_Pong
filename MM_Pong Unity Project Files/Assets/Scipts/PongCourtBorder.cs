using UnityEngine;

public class PongCourtBorder : MonoBehaviour
{
    //SFX
    protected AudioSource ballHitsWallSFX;

    void Start()
    {
        ballHitsWallSFX = GetComponent<AudioSource>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            HandleBallBounce();
        }
    }

    private void HandleBallBounce()
    {
        if (ballHitsWallSFX)
        {
            ballHitsWallSFX.Play();
        }
    }
}
