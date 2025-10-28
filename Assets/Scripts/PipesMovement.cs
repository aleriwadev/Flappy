using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PipesMovement : MonoBehaviour
{
    public float speed = 2f;
    private float leftEdge;
    //private float bgLeft = -12;

    private void Start()
    {
        leftEdge = Camera.main.ScreenToWorldPoint(Vector2.zero).x - 2f;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
        //transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < leftEdge)
        {
            Destroy(gameObject);
        }
    }
}
