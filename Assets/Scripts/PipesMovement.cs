using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipesMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    private float currentSpeed;
    private float leftEdge;

    [Header("Movement Pattern (Optional)")]
    public bool useVerticalMovement = false;
    public float verticalSpeed = 1f;
    public float verticalRange = 2f;
    private float startY;
    private float verticalDirection = 1f;

    private void Start()
    {
        leftEdge = Camera.main.ScreenToWorldPoint(Vector2.zero).x - 2f;
        startY = transform.position.y;

        // Get initial speed from LevelManager
        UpdateSpeed();

        // Subscribe to level changes
        if (LevelManager.Instance != null)
        {
            LevelManager.OnDifficultyChanged += OnDifficultyChanged;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe when pipe is destroyed
        if (LevelManager.Instance != null)
        {
            LevelManager.OnDifficultyChanged -= OnDifficultyChanged;
        }
    }

    void OnDifficultyChanged(float speed, float gapSize, float powerUpInterval)
    {
        // Update speed when level changes
        currentSpeed = speed;
    }

    void UpdateSpeed()
    {
        if (LevelManager.Instance != null)
        {
            currentSpeed = LevelManager.Instance.CurrentSpeed;
        }
        else
        {
            currentSpeed = speed;
        }
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameActive) return;

        // Horizontal movement with current speed
        transform.position += Vector3.left * currentSpeed * Time.deltaTime;

        // Optional vertical movement pattern
        if (useVerticalMovement)
        {
            VerticalMovement();
        }

        // Destroy when off screen
        if (transform.position.x < leftEdge)
        {
            Destroy(gameObject);
        }
    }

    void VerticalMovement()
    {
        transform.position += Vector3.up * verticalSpeed * verticalDirection * Time.deltaTime;

        // Reverse direction when reaching range limits
        if (transform.position.y > startY + verticalRange)
        {
            verticalDirection = -1f;
        }
        else if (transform.position.y < startY - verticalRange)
        {
            verticalDirection = 1f;
        }
    }
}