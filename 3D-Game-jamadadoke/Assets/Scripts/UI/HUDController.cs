using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI comboText;
    [SerializeField] TextMeshProUGUI innocentCountText;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TextMeshProUGUI gameOverReasonText;
    [SerializeField] Button restartButton;

    void Start()
    {
        gameOverPanel.SetActive(false);

        ScoreManager.Instance.OnScoreChanged += RefreshScore;
        GameManager.Instance.OnGameOver += ShowGameOver;

        restartButton.onClick.AddListener(GameManager.Instance.Restart);

        RefreshScore(0, 0, 1f);
        RefreshInnocent();
    }

    void RefreshScore(int score, int combo, float multiplier)
    {
        scoreText.text = $"SCORE  {score:N0}";
        comboText.text = combo > 1 ? $"COMBO x{combo}  ({multiplier:F1}x)" : "";
        RefreshInnocent();
    }

    void RefreshInnocent()
    {
        int killed = GameManager.Instance.InnocentKilled;
        int limit  = GameManager.Instance.InnocentKillLimit;
        innocentCountText.text = $"誤爆  {killed} / {limit}";
        innocentCountText.color = killed >= limit - 1 ? Color.red : Color.white;
    }

    void ShowGameOver(GameManager.GameState _, string reason)
    {
        gameOverPanel.SetActive(true);
        gameOverReasonText.text = reason;
    }
}
