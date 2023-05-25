using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;
    public GameObject structureMenuUI;
    public GameObject victoryMenuUI;
    public GameObject tutorialMenuUI;
    public GameObject defeatMenuUI;
    public GameObject miniMap;

    Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;


    // DOESNT REALLY PAUSE THE GAME (TRUE CONTROL IS IN GAMECONTROLLER)

    private void Start()
    {
        resolutions = Screen.resolutions;

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for(int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width >= 1400 && (!options.Contains(resolutions[i].width.ToString()) || !options.Contains(resolutions[i].height.ToString())))
            {
                string option = resolutions[i].width + "x" + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
            else
            {
                continue;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void Resume()
    {
        // null checking, errors if otherwise
        if (pauseMenuUI != null)
        pauseMenuUI.SetActive(false);
       
        if (optionsMenuUI != null)
        optionsMenuUI.SetActive(false);

        if (structureMenuUI != null)
            structureMenuUI.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        //pauseMenuUI.SetActive(true);
        optionsMenuUI.SetActive(false);
        miniMap.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void Victory()
    {
        if (victoryMenuUI == null || optionsMenuUI == null)
        {
            return;
        }

        victoryMenuUI.SetActive(true);
        miniMap.SetActive(false);
        optionsMenuUI.SetActive(false);
        tutorialMenuUI.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void Defeat()
    {
        defeatMenuUI.SetActive(true);
        optionsMenuUI.SetActive(false);
        miniMap.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;

    }

    public void NewGame()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
        Resume();
    }

    public void LoadOptions()
    {
        SceneManager.LoadScene("OptionsMenu");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void InGameSettings()
    {
        optionsMenuUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void SetVolume(float volume)
    {
        Debug.Log(volume);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void StructureMenu()
    {
        structureMenuUI.SetActive(true);
        GameIsPaused = true;
        Time.timeScale = 0f;
    }
    public void LoadTutorial()
    {
        SceneManager.LoadScene("Tutorial", LoadSceneMode.Single);
    }
    public void LoadMode()
    {
        SceneManager.LoadScene("Mode", LoadSceneMode.Single);
    }
}
