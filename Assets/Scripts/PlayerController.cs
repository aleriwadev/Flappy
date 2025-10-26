using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveForce = 1f;
    [SerializeField]
    private float flyForce = 15f;

    private Rigidbody2D playerRb;
    private Animator playerAnimator;

    private string FLY_ANIMATION = "fly";
    SpriteRenderer playerRenderer;

    private Vector2 direction;

    //bool fly;

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
        direction = Vector2.zero;
    }


    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        //transform.Translate(Vector2.right * moveForce * Time.deltaTime);
        AnimatePlayer();
    }

    void PlayerMovement()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            direction = Vector2.up * flyForce;
           // playerRb.AddForce(Vector2.up * flyForce * Time.deltaTime, ForceMode2D.Impulse);
        }
    }

    void AnimatePlayer()
    {
        playerAnimator.SetBool(FLY_ANIMATION, true);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            FindObjectOfType<GameManager>().GameOver();          
        }
        else if(collision.gameObject.tag == "Scoring")
        {
            FindObjectOfType<GameManager>().IncresaseScore();
        }
    }
}
