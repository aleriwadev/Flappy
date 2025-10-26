using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public PlayerController player;
    private int score;
    public TextMeshProUGUI scoreText;
    public Text scoreTxt;
    public GameObject playButton;
    public GameObject gameOver;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Pause();
    }

    void Play()
    {
        score = 0;
        scoreText.text = score.ToString();

        playButton.SetActive(false);
        gameOver.SetActive(false);

        Time.timeScale = 1f;
        player.enabled = true;

        // Destroy all the pipes on the scene before a game
        PipesMovement[] pipes = FindObjectsOfType<PipesMovement>();
        for(int i = 0; i < pipes.Length; i++)
        {
            Destroy(pipes[i].gameObject);
        }
    }

    void Pause()
    {
        Time.timeScale = 0;
        player.enabled = false;
    }
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
        
        score++;
        scoreText.text = score.ToString();
        //Check this if it works
        //scoreTxt.text = score.ToString();
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        gameOver.SetActive(true);
        playButton.SetActive(true);

        Pause();
    }

}
