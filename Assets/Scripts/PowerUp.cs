using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Settings")]
    public PowerUpType type;
    public float moveSpeed = 2f;
    public float lifetime = 10f; // Auto-destroy after this time

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    private float leftEdge;
    private float spawnTime;
    private bool collected = false;

    private void Start()
    {
        leftEdge = Camera.main.ScreenToWorldPoint(Vector2.zero).x - 2f;
        spawnTime = Time.time;
    }

    private void Update()
    {
        // Don't move if game isn't active
        if (GameManager.Instance == null || !GameManager.Instance.IsGameActive)
        {
            return;
        }

        // Move left with obstacles
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        // Optional: Add floating animation
        float floatOffset = Mathf.Sin(Time.time * 3f) * 0.2f;
        transform.position = new Vector3(transform.position.x, transform.position.y + floatOffset * Time.deltaTime, transform.position.z);

        // Destroy if off-screen or lifetime expired
        if (transform.position.x < leftEdge || Time.time - spawnTime > lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected) return;

        if (collision.CompareTag("Player"))
        {
            collected = true;
            Collect();
        }
    }

    void Collect()
    {
        // Notify PowerUpManager
        PowerUpManager.Instance.ActivatePowerUp(type);

        // Play collection sound
        if (AudioManager.Instance != null)
        {
            PowerUpData data = PowerUpManager.Instance.GetPowerUpData(type);
            if (data != null)
            {
                AudioManager.Instance.Play(data.collectSoundName);
            }
        }

        // Spawn particle effect at position before destroying
        PowerUpData powerUpData = PowerUpManager.Instance.GetPowerUpData(type);
        if (powerUpData != null && powerUpData.particleEffect != null)
        {
            GameObject particles = Instantiate(powerUpData.particleEffect, transform.position, Quaternion.identity);
            Destroy(particles, 2f);
        }

        // Destroy the power-up
        Destroy(gameObject);
    }

    public void Initialize(PowerUpType powerUpType, Sprite icon, Color color)
    {
        type = powerUpType;

        if (spriteRenderer != null)
        {
            // Set the sprite ONLY if an icon is provided
            if (icon != null)
            {
                spriteRenderer.sprite = icon;
            }

            // Always set the color
            spriteRenderer.color = color;

            Debug.Log($"PowerUp initialized: {type}, Icon: {(icon != null ? icon.name : "NULL")}, Color: {color}");
        }
        else
        {
            Debug.LogError("SpriteRenderer is NULL on PowerUp!");
        }
    }
}