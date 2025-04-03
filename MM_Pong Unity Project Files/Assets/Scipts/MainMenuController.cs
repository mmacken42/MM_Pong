using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string gameplayLevelName;
    public void StartGame()
    {
        SceneManager.LoadScene(gameplayLevelName);
    }
}
