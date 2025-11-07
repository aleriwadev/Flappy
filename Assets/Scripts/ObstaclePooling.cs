using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePooling : MonoBehaviour
{
    public GameObject prefab;
    public float spawnRate = 2f;
    public float minHeight = -1f;
    public float maxHeight = 1f;

    // Gap size (distance between top and bottom pipe)
    [Header("Pipe Gap")]
    public float defaultGapSize = 3f;
    private float currentGapSize;

    private void OnEnable()
    {
        InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);

        // Subscribe to level changes
        LevelManager.OnDifficultyChanged += OnDifficultyChanged;

        // Set initial gap size
        if (LevelManager.Instance != null)
        {
            currentGapSize = LevelManager.Instance.CurrentGapSize;
        }
        else
        {
            currentGapSize = defaultGapSize;
        }
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Spawn));
        LevelManager.OnDifficultyChanged -= OnDifficultyChanged;
    }

    void OnDifficultyChanged(float speed, float gapSize, float powerUpInterval)
    {
        currentGapSize = gapSize;
        Debug.Log($"ObstaclePooling: Gap size updated to {gapSize}");
    }

    public void Spawn()
    {
        GameObject pipes = Instantiate(prefab, transform.position, Quaternion.identity);
        pipes.transform.position += Vector3.up * Random.Range(minHeight, maxHeight);

        // Adjust gap size if the prefab has adjustable pipes
        // This assumes your pipe prefab has top and bottom pipes as children
        AdjustPipeGap(pipes);
    }

    void AdjustPipeGap(GameObject pipeSet)
    {
        // Find top and bottom pipes (assumes they're named "Top" and "Bottom" or similar)
        Transform topPipe = pipeSet.transform.Find("Top");
        Transform bottomPipe = pipeSet.transform.Find("Bottom");

        if (topPipe != null && bottomPipe != null)
        {
            // Position pipes based on current gap size
            float halfGap = currentGapSize / 2f;
            topPipe.localPosition = new Vector3(0, halfGap, 0);
            bottomPipe.localPosition = new Vector3(0, -halfGap, 0);
        }
    }
}