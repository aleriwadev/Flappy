using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance { get; private set; }

    [Header("Spawning")]
    public GameObject powerUpPrefab;
    public float spawnInterval = 15f; // Spawn every 15 seconds
    public float minHeight = -2f;
    public float maxHeight = 2f;
    public Vector3 spawnPosition = new Vector3(10f, 0f, 0f);

    [Header("Power-Up Definitions")]
    public PowerUpData[] powerUpTypes;

    [Header("Active Power-Ups")]
    private Dictionary<PowerUpType, Coroutine> activePowerUps = new Dictionary<PowerUpType, Coroutine>();
    private Dictionary<PowerUpType, float> powerUpTimers = new Dictionary<PowerUpType, float>();

    // Score multiplier
    private int scoreMultiplier = 1;
    public int ScoreMultiplier => scoreMultiplier;

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
        GameManager.OnGameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= OnGameStart;
        GameManager.OnGameOver -= OnGameOver;
    }

    private void OnGameStart()
    {
        // Start spawning power-ups
        InvokeRepeating(nameof(SpawnRandomPowerUp), spawnInterval, spawnInterval);
    }

    private void OnGameOver()
    {
        // Stop spawning
        CancelInvoke(nameof(SpawnRandomPowerUp));

        // Clear all active power-ups
        DeactivateAllPowerUps();
    }

    void SpawnRandomPowerUp()
    {
        if (powerUpTypes.Length == 0) return;

        // Calculate total spawn chance
        float totalChance = 0f;
        foreach (var powerUp in powerUpTypes)
        {
            totalChance += powerUp.spawnChance;
        }

        // Random selection based on spawn chance weights
        float randomValue = Random.Range(0f, totalChance);
        float cumulative = 0f;

        PowerUpData selectedPowerUp = null;
        foreach (var powerUp in powerUpTypes)
        {
            cumulative += powerUp.spawnChance;
            if (randomValue <= cumulative)
            {
                selectedPowerUp = powerUp;
                break;
            }
        }

        if (selectedPowerUp != null)
        {
            SpawnPowerUp(selectedPowerUp);
        }
    }

    void SpawnPowerUp(PowerUpData data)
    {
        // Random height
        Vector3 spawnPos = spawnPosition;
        spawnPos.y = Random.Range(minHeight, maxHeight);

        // Instantiate power-up
        GameObject powerUpObj = Instantiate(powerUpPrefab, spawnPos, Quaternion.identity);
        PowerUp powerUpScript = powerUpObj.GetComponent<PowerUp>();

        if (powerUpScript != null)
        {
            // Initialize with the specific power-up's icon and color
            powerUpScript.Initialize(data.type, data.icon, data.color);

            // Debug to see what's spawning
            Debug.Log($"Spawned {data.type} power-up at {spawnPos} with icon: {(data.icon != null ? data.icon.name : "NULL")}");
        }
    }

    public void ActivatePowerUp(PowerUpType type)
    {
        PowerUpData data = GetPowerUpData(type);
        if (data == null) return;

        // If already active, restart the timer
        if (activePowerUps.ContainsKey(type))
        {
            StopCoroutine(activePowerUps[type]);
        }

        // Start power-up effect
        Coroutine coroutine = StartCoroutine(PowerUpCoroutine(type, data.duration));
        activePowerUps[type] = coroutine;
        powerUpTimers[type] = data.duration;

        // Apply power-up effect
        ApplyPowerUpEffect(type, true);

        // Play activation sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(data.activateSoundName);
        }

        // Notify UI
        if (PowerUpUI.Instance != null)
        {
            PowerUpUI.Instance.ShowPowerUp(type, data.duration, data.icon);
        }
    }

    IEnumerator PowerUpCoroutine(PowerUpType type, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            powerUpTimers[type] = duration - elapsed;
            yield return null;
        }

        // Power-up expired
        DeactivatePowerUp(type);
    }

    void DeactivatePowerUp(PowerUpType type)
    {
        if (!activePowerUps.ContainsKey(type)) return;

        // Remove power-up effect
        ApplyPowerUpEffect(type, false);

        // Clean up
        activePowerUps.Remove(type);
        powerUpTimers.Remove(type);

        // Play expire sound
        PowerUpData data = GetPowerUpData(type);
        if (data != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.Play(data.expireSoundName);
        }

        // Notify UI
        if (PowerUpUI.Instance != null)
        {
            PowerUpUI.Instance.HidePowerUp(type);
        }
    }

    void ApplyPowerUpEffect(PowerUpType type, bool activate)
    {
        switch (type)
        {
            case PowerUpType.Shield:
                if (GameManager.Instance.player != null)
                {
                    GameManager.Instance.player.SetInvincibility(activate);
                }
                break;

            case PowerUpType.SlowMotion:
                Time.timeScale = activate ? 0.5f : 1f;
                break;

            case PowerUpType.DoublePoints:
                scoreMultiplier = activate ? 2 : 1;
                break;

            case PowerUpType.SpeedBoost:
                if (GameManager.Instance.player != null)
                {
                    GameManager.Instance.player.SetSpeedMultiplier(activate ? 1.5f : 1f);
                }
                break;
        }
    }

    void DeactivateAllPowerUps()
    {
        // Stop all coroutines and reset effects
        List<PowerUpType> activeTypes = new List<PowerUpType>(activePowerUps.Keys);

        foreach (var type in activeTypes)
        {
            if (activePowerUps.ContainsKey(type))
            {
                StopCoroutine(activePowerUps[type]);
            }
            ApplyPowerUpEffect(type, false);
        }

        activePowerUps.Clear();
        powerUpTimers.Clear();
        scoreMultiplier = 1;

        // Reset time scale
        Time.timeScale = 1f;

        // Reset player
        if (GameManager.Instance.player != null)
        {
            GameManager.Instance.player.ResetPowerUpEffects();
        }
    }

    public PowerUpData GetPowerUpData(PowerUpType type)
    {
        foreach (var data in powerUpTypes)
        {
            if (data.type == type)
                return data;
        }
        return null;
    }

    public bool IsPowerUpActive(PowerUpType type)
    {
        return activePowerUps.ContainsKey(type);
    }

    public float GetPowerUpTimeRemaining(PowerUpType type)
    {
        if (powerUpTimers.ContainsKey(type))
            return powerUpTimers[type];
        return 0f;
    }
}