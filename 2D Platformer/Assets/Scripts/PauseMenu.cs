﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public Rigidbody2D playerRigidbody;

    private Vector2 savedVelocity;
    private float savedAngularVelocity; 

    private void Start()
    {
        pauseMenuUI.SetActive(false);

        if (playerRigidbody == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerRigidbody = player.GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;

        if (playerRigidbody != null)
        {
            playerRigidbody.simulated = true; 
            playerRigidbody.linearVelocity = savedVelocity;
            playerRigidbody.angularVelocity = savedAngularVelocity;
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

        if (playerRigidbody != null)
        {
            savedVelocity = playerRigidbody.linearVelocity;
            savedAngularVelocity = playerRigidbody.angularVelocity;
            playerRigidbody.simulated = false;
        }
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;

        if (playerRigidbody != null)
            playerRigidbody.simulated = true;

        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        if (playerRigidbody != null)
            playerRigidbody.simulated = true;

        SceneManager.LoadScene(1);
    }
}