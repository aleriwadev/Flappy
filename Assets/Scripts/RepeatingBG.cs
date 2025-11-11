using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatingBG : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speedMultiplier = 0.5f; // Background moves at 50% of pipe speed
    private float currentSpeed;

    [SerializeField]
    private Renderer bgRenderer;

    private void Start()
    {
        // Subscribe to level changes
        if (LevelManager.Instance != null)
        {
            LevelManager.OnDifficultyChanged += OnDifficultyChanged;
        }

        UpdateSpeed();
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.OnDifficultyChanged -= OnDifficultyChanged;
        }
    }

    void OnDifficultyChanged(float pipeSpeed, float gapSize, float powerUpInterval)
    {
        // Update background speed based on pipe speed
        currentSpeed = pipeSpeed * speedMultiplier;
    }

    void UpdateSpeed()
    {
        if (LevelManager.Instance != null)
        {
            currentSpeed = LevelManager.Instance.CurrentSpeed * speedMultiplier;
        }
        else
        {
            currentSpeed = 2f * speedMultiplier; // Default
        }
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameActive) return;

        bgRenderer.material.mainTextureOffset += new Vector2(currentSpeed * Time.deltaTime, 0);
    }
}