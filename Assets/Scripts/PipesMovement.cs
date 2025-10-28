using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipesMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
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
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameActive) return;

        // Horizontal movement
        transform.position += Vector3.left * speed * Time.deltaTime;

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