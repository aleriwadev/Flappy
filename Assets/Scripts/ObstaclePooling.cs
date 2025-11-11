using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePooling : MonoBehaviour
{
    public GameObject prefab;
    public float spawnRate = 2f; // Base spawn rate
    private float currentSpawnRate;
    public float minHeight = -1f;
    public float maxHeight = 1f;

    private void OnEnable()
    {
        // Set initial spawn rate
        currentSpawnRate = spawnRate;
        InvokeRepeating(nameof(Spawn), currentSpawnRate, currentSpawnRate);

        // Subscribe to level changes
        if (LevelManager.Instance != null)
        {
            LevelManager.OnDifficultyChanged += OnDifficultyChanged;
        }
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Spawn));

        if (LevelManager.Instance != null)
        {
            LevelManager.OnDifficultyChanged -= OnDifficultyChanged;
        }
    }

    void OnDifficultyChanged(float speed, float gapSize, float powerUpInterval)
    {
        // Update spawn rate based on pipe speed
        // As speed increases, spawn rate should decrease (spawn faster)
        // Formula: faster pipes = need more frequent obstacles
        currentSpawnRate = spawnRate / (speed / 2f); // 2f is base speed
        currentSpawnRate = Mathf.Max(currentSpawnRate, 1f); // Never faster than 1 second

        // Restart spawning with new rate
        CancelInvoke(nameof(Spawn));
        InvokeRepeating(nameof(Spawn), currentSpawnRate, currentSpawnRate);

        Debug.Log($"ObstaclePooling: Spawn rate updated to {currentSpawnRate}s");
    }

    public void Spawn()
    {
        GameObject pipes = Instantiate(prefab, transform.position, Quaternion.identity);
        pipes.transform.position += Vector3.up * Random.Range(minHeight, maxHeight);
    }
}