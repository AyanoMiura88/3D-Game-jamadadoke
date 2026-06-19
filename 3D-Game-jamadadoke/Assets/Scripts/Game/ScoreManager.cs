using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int Score { get; private set; }
    public int Combo { get; private set; }
    public float ComboMultiplier => 1f + Combo * 0.1f;

    public event Action<int, int, float> OnScoreChanged; // score, combo, multiplier

    const int BASE_HIT_SCORE = 100;
    const int BASE_THROW_SCORE = 150;
    const int BASE_CHAIN_SCORE = 200;
    const float COMBO_RESET_TIME = 3f;

    float comboTimer;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (Combo <= 0) return;
        comboTimer -= Time.deltaTime;
        if (comboTimer <= 0f)
            ResetCombo();
    }

    public void AddHitScore()    => AddScore(BASE_HIT_SCORE);
    public void AddThrowScore()  => AddScore(BASE_THROW_SCORE);
    public void AddChainScore()  => AddScore(BASE_CHAIN_SCORE);

    void AddScore(int baseScore)
    {
        Combo++;
        comboTimer = COMBO_RESET_TIME;
        int gained = Mathf.RoundToInt(baseScore * ComboMultiplier);
        Score += gained;
        OnScoreChanged?.Invoke(Score, Combo, ComboMultiplier);
    }

    public void ResetCombo()
    {
        Combo = 0;
        comboTimer = 0f;
        OnScoreChanged?.Invoke(Score, Combo, ComboMultiplier);
    }
}
