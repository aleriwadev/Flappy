using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public PlayerController player;
    public TextMeshProUGUI scoreText;
    public GameObject playButton;
    public GameObject gameOver;

    [Header("Game State")]
    private int score;
    private bool isGameActive;

    // Events for other systems to listen to
    public static event Action OnGameStart;
    public static event Action OnGameOver;
    public static event Action<int> OnScoreChanged;
    public static event Action OnPause;
    public static event Action OnResume;

    [Header("Game Settings")]
    [SerializeField] private float gameOverDelay = 0.5f; // Delay before showing game over UI
    [SerializeField] private float restartDelay = 0.3f;  // Delay before game actually starts

    // Properties for other scripts to access
    public bool IsGameActive => isGameActive;
    public int CurrentScore => score;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Application.targetFrameRate = 60;
        Pause();
    }

    public void Play()
    {
        score = 0;
        scoreText.text = $"Score: {score}";
        playButton.SetActive(false);
        gameOver.SetActive(false);

        Time.timeScale = 1f;
        isGameActive = true;
        player.enabled = true;

        // Clean up pipes
        PipesMovement[] pipes = FindObjectsOfType<PipesMovement>();
        for (int i = 0; i < pipes.Length; i++)
        {
            Destroy(pipes[i].gameObject);
        }

        // Notify all systems that game has started
        OnGameStart?.Invoke();
    }

    public void Pause()
    {
        Time.timeScale = 0;
        isGameActive = false;
        player.enabled = false;
        OnPause?.Invoke();
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        isGameActive = true;
        player.enabled = true;
        OnResume?.Invoke();
    }

    public void IncreaseScore(int amount = 1)
    {
        if (!isGameActive) return;

        score += amount;
        scoreText.text = $"Score: {score}";

        // Notify systems about score change (for power-ups, levels, etc.)
        OnScoreChanged?.Invoke(score);
    }

    public void GameOver()
    {
        if (!isGameActive) return;

        Debug.Log("Game Over");
        isGameActive = false;
        gameOver.SetActive(true);
        playButton.SetActive(true);

        Pause();

        // Notify all systems
        OnGameOver?.Invoke();
    }

    private void OnDestroy()
    {
        // Clean up singleton
        if (Instance == this)
        {
            Instance = null;
        }
    }
}