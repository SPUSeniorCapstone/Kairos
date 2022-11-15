using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController main;

    public bool paused;

    public PlayerController playerController;
    public GameObject pauseMenu;
    [SerializeField] public Hero hero;

    public CursorLockMode defaultLockMode = CursorLockMode.None;

    private void Awake()
    {
        if (main != null && main != this)
        {
            Debug.LogWarning("Cannot have more than one GameController in a scene");
        }
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
                Cursor.lockState = defaultLockMode;
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
                paused = true;

            }
        }

        if (Input.GetKeyDown(KeyCode.F)) // removed need for hero to be selected
        {
            Debug.Log("F");
            if (hero.GetComponentInChildren<Unit>() != null)
            {
                hero.GetComponentInChildren<Unit>().enabled = !hero.GetComponentInChildren<Unit>().enabled;
            }
            hero.smallCamera.GetComponent<Camera>().enabled = !hero.smallCamera.GetComponent<Camera>().enabled;
            hero.smallCamera.GetComponent<HeroCamera>().enabled = !hero.smallCamera.GetComponent<HeroCamera>().enabled;
            hero.RTSCamera.GetComponent<Camera>().enabled = !hero.RTSCamera.GetComponent<Camera>().enabled;
            hero.RTSCamera.GetComponent<CameraController>().enabled = !hero.RTSCamera.GetComponent<CameraController>().enabled;

            hero.controlled = !hero.controlled;
            hero.enabled = !hero.enabled;
            hero.GetComponent<CharacterController>().enabled = !hero.GetComponent<CharacterController>().enabled;
        }
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
