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

    bool fly;

    private void Awake()
    {
        playerRenderer = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody2D>();
        
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
            playerRb.velocity = Vector2.up * flyForce;
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
