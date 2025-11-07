using System;
using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Settings")]
    [SerializeField] private int pointsPerLevel = 5; // Score needed to level up
    [SerializeField] private int maxLevel = 10; // Maximum level (difficulty caps here)

    [Header("Difficulty Scaling")]
    [SerializeField] private float baseSpeed = 2f; // Starting pipe speed
    [SerializeField] private float speedIncreasePerLevel = 0.3f; // Speed increase each level
    [SerializeField] private float maxSpeed = 5f; // Maximum speed cap

    [Header("Pipe Gap Scaling")]
    [SerializeField] private float baseGapSize = 3f; // Starting gap between pipes
    [SerializeField] private float gapDecreasePerLevel = 0.15f; // Gap decrease each level
    [SerializeField] private float minGapSize = 1.5f; // Minimum gap size

    [Header("Power-Up Spawn Scaling")]
    [SerializeField] private float basePowerUpInterval = 15f; // Starting spawn interval
    [SerializeField] private float powerUpIntervalDecrease = 1f; // Faster spawning each level
    [SerializeField] private float minPowerUpInterval = 8f; // Fastest spawn rate

    [Header("Current State")]
    private int currentLevel = 1;
    private int scoreAtLastLevel = 0;

    // Events
    public static event Action<int> OnLevelUp;
    public static event Action<float, float, float> OnDifficultyChanged; // speed, gap, powerUpInterval

    // Properties for other systems to access
    public int CurrentLevel => currentLevel;
    public float CurrentSpeed => CalculateSpeed();
    public float CurrentGapSize => CalculateGapSize();
    public float CurrentPowerUpInterval => CalculatePowerUpInterval();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameStart += OnGameStart;
        GameManager.OnScoreChanged += OnScoreChanged;
        GameManager.OnGameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= OnGameStart;
        GameManager.OnScoreChanged -= OnScoreChanged;
        GameManager.OnGameOver -= OnGameOver;
    }

    private void OnGameStart()
    {
        // Reset to level 1
        currentLevel = 1;
        scoreAtLastLevel = 0;

        // Apply initial difficulty
        ApplyDifficulty();
    }

    private void OnScoreChanged(int newScore)
    {
        // Check if player has scored enough for next level
        int levelThreshold = currentLevel * pointsPerLevel;

        if (newScore >= levelThreshold && currentLevel < maxLevel)
        {
            LevelUp();
        }
    }

    private void OnGameOver()
    {
        // Could save high level here if needed
    }

    void LevelUp()
    {
        currentLevel++;
        scoreAtLastLevel = GameManager.Instance.CurrentScore;

        Debug.Log($"Level Up! Now at Level {currentLevel}");

        // Apply new difficulty
        ApplyDifficulty();

        // Play level up sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play("levelup");
        }

        // Notify other systems
        OnLevelUp?.Invoke(currentLevel);
    }

    void ApplyDifficulty()
    {
        float speed = CalculateSpeed();
        float gapSize = CalculateGapSize();
        float powerUpInterval = CalculatePowerUpInterval();

        Debug.Log($"Level {currentLevel}: Speed={speed:F2}, Gap={gapSize:F2}, PowerUp Interval={powerUpInterval:F1}");

        // Notify systems of new difficulty
        OnDifficultyChanged?.Invoke(speed, gapSize, powerUpInterval);
    }

    float CalculateSpeed()
    {
        float speed = baseSpeed + (currentLevel - 1) * speedIncreasePerLevel;
        return Mathf.Min(speed, maxSpeed);
    }

    float CalculateGapSize()
    {
        float gap = baseGapSize - (currentLevel - 1) * gapDecreasePerLevel;
        return Mathf.Max(gap, minGapSize);
    }

    float CalculatePowerUpInterval()
    {
        float interval = basePowerUpInterval - (currentLevel - 1) * powerUpIntervalDecrease;
        return Mathf.Max(interval, minPowerUpInterval);
    }

    // Helper method for UI to show progress to next level
    public float GetLevelProgress()
    {
        int currentScore = GameManager.Instance.CurrentScore;
        int scoreNeeded = pointsPerLevel;
        int scoreInCurrentLevel = currentScore - scoreAtLastLevel;

        return Mathf.Clamp01((float)scoreInCurrentLevel / scoreNeeded);
    }

    public int GetScoreUntilNextLevel()
    {
        int currentScore = GameManager.Instance.CurrentScore;
        int nextLevelThreshold = currentLevel * pointsPerLevel;
        return nextLevelThreshold - currentScore;
    }
}