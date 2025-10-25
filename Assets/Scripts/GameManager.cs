using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int score;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncresaseScore()
    {
        Debug.Log("Scored 1");
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
    }

    private void OnCollisionEnter3D(Collision collision)
    {
        
    }
}
