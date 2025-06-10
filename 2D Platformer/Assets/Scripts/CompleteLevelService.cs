using UnityEngine.SceneManagement;
using UnityEngine;

public class CompleteLevelService : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int levelID = GetSceneID(SceneManager.GetActiveScene().name);
            if (levelID == 0) return;

            int stars = CalculateStars(Hero.Instance.currentScore);
            GameService.Instance.CompleteLevel(
                PlayerPrefs.GetInt("id"),
                stars,
                Hero.Instance.currentScore,
                levelID
            );
        }
    }

    private int CalculateStars(int score)
    {
        if (score >= 1000) return 3;
        if (score >= 500) return 2;
        return 1;
    }

    private int GetSceneID(string sceneName)
    {
        return sceneName switch
        {
            "Level1" => 1,
            "Level2" => 2,
            "Level3" => 3,
            _ => 0
        };
    }
}