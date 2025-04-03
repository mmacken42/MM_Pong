using UnityEngine;

public class PongScoreTrigger : MonoBehaviour
{
    public PongGameManager gameManager;
    public bool isPlayerOneSideOfCourt;

    //SFX
    private AudioSource scoreSFX;

    void Start()
    {
        scoreSFX = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (scoreSFX)
        {
            scoreSFX.Play();
        }
        
        if (isPlayerOneSideOfCourt)
        {
            gameManager.IncScorePlayerTwo();
        }
        else
        {
            gameManager.IncScorePlayerOne();
        }
    }
}
