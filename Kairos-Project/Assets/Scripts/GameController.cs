using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController main;

    public bool paused;

    public PlayerController playerController;
    public GameObject pauseMenu;

    private void Awake()
    {
        main = this; 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 1;
                pauseMenu.SetActive(false);
                paused = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
                paused = true;

            }
        }
    }
}
