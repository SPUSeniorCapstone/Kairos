using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController main;

    public bool paused;
    [DisableOnPlay(true)]
    public bool heroMode;

    public PlayerController playerController;
    public GameObject pauseMenu;
    [SerializeField] public Hero hero;
    public CameraController cameraController;

    public CursorLockMode defaultLockMode = CursorLockMode.None;

    private void Awake()
    {
        if (main != null && main != this)
        {
            Debug.LogWarning("Cannot have more than one GameController in a scene");
        }
        main = this;
    }

    private void Start()
    {
        //Inital mode is always non-hero - This will swap it if the initial state should be hero mode
        if (heroMode)
        {
            heroMode = false;
            SwapMode(true);
        }
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

        if (Input.GetKeyDown(KeyCode.F)) 
        {
            SwapMode(!heroMode);
        }

    }

    public void SwapMode(bool useHero)
    {
        if(useHero == heroMode)
        {
            return;
        }

        if (hero.GetComponentInChildren<Unit>() != null)
        {
            hero.GetComponentInChildren<Unit>().enabled = !hero.GetComponentInChildren<Unit>().enabled;
        }

        hero.controlled = !hero.controlled;
        hero.enabled = !hero.enabled;
        hero.GetComponent<CharacterController>().enabled = !hero.GetComponent<CharacterController>().enabled;

        // Moved Camera swapping to Camera Controller
        if (hero.controlled == true)
        {
            cameraController.SwapCamera(CameraController.CameraMode.HERO_THIRD_PERSON);
        }
        else
        {
            cameraController.SwapCamera(CameraController.CameraMode.FREE);
        }

        heroMode = useHero;
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
