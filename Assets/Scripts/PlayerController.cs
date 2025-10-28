using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float flyForce = 15f;
    [SerializeField] private float maxVelocity = 10f;

    [Header("Components")]
    private Rigidbody2D playerRb;
    private Animator playerAnimator;
    private SpriteRenderer playerRenderer;

    [Header("State")]
    private bool isInvincible = false;
    private float currentSpeedMultiplier = 1f;

    private const string FLY_ANIMATION = "fly";

    // Properties for power-ups to modify
    public bool IsInvincible => isInvincible;
    public float SpeedMultiplier => currentSpeedMultiplier;

    private void Awake()
    {
        playerRenderer = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        Vector2 position = transform.position;
        position.y = 0f;
        transform.position = position;
        playerRb.velocity = Vector2.zero;

        // Reset power-up effects
        isInvincible = false;
        currentSpeedMultiplier = 1f;
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameActive) return;

        PlayerMovement();
        AnimatePlayer();
        ClampVelocity();
    }

    void PlayerMovement()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            // Reset velocity before applying force
            playerRb.velocity = Vector2.zero;
            // Apply force with speed multiplier (for slow-mo power-up)
            playerRb.AddForce(Vector2.up * flyForce * currentSpeedMultiplier, ForceMode2D.Impulse);
        }
    }

    void ClampVelocity()
    {
        // Prevent crazy speeds
        if (playerRb.velocity.magnitude > maxVelocity)
        {
            playerRb.velocity = playerRb.velocity.normalized * maxVelocity;
        }
    }

    void AnimatePlayer()
    {
        playerAnimator.SetBool(FLY_ANIMATION, true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (!isInvincible)
            {
                GameManager.Instance.GameOver();
            }
            else
            {
                Debug.Log("Hit obstacle but invincible!");
            }
        }
        else if (collision.gameObject.CompareTag("Scoring"))
        {
            GameManager.Instance.IncreaseScore();
        }
        else if (collision.gameObject.CompareTag("PowerUp"))
        {
            // Power-up system will handle this
            collision.gameObject.SetActive(false);
        }
    }

    // Power-up methods
    public void SetInvincibility(bool invincible)
    {
        isInvincible = invincible;
        // Visual feedback - make player semi-transparent when invincible
        Color color = playerRenderer.color;
        color.a = invincible ? 0.5f : 1f;
        playerRenderer.color = color;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        currentSpeedMultiplier = multiplier;
    }

    public void ResetPowerUpEffects()
    {
        SetInvincibility(false);
        SetSpeedMultiplier(1f);
    }
}