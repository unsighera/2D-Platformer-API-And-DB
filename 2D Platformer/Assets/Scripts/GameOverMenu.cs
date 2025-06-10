using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private GameObject gameOverMenuUI;
    [SerializeField] private Rigidbody2D playerRigidbody;

    public static GameOverMenu Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (gameOverMenuUI != null)
            gameOverMenuUI.SetActive(false);
    }

    public void ActivateGameOver()
    {
        Time.timeScale = 0f;

        if (playerRigidbody != null)
            playerRigidbody.simulated = false;

        if (gameOverMenuUI != null)
            gameOverMenuUI.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Hero.Instance.health = 5;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(0);
    }
}