using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Playing, GameOver }
    public GameState State { get; private set; } = GameState.Playing;

    [Header("Rules")]
    [SerializeField] int innocentKillLimit = 3;

    public int InnocentKilled { get; private set; }
    public int InnocentKillLimit => innocentKillLimit;

    public event Action<GameState, string> OnGameOver;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ReportInnocentKilled()
    {
        if (State != GameState.Playing) return;
        InnocentKilled++;
        if (InnocentKilled >= innocentKillLimit)
            TriggerGameOver("駅員に取り押さえられた…");
    }

    public void TriggerGameOver(string reason)
    {
        if (State != GameState.Playing) return;
        State = GameState.GameOver;
        OnGameOver?.Invoke(State, reason);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
