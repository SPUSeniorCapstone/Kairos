using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    UIDocument document;
    public GameObject minimap;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        var exitButton = document.rootVisualElement.Q("ExitButton") as Button;
        exitButton.RegisterCallback<ClickEvent>(ExitToMainMenu);

        var startButton = document.rootVisualElement.Q("StartButton") as Button;
        startButton.RegisterCallback<ClickEvent>(StartGame);

    }

    public void ExitToMainMenu(ClickEvent click)
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void StartGame(ClickEvent click)
    {
        SceneManager.LoadScene("Game");
    }

    void Update()
    {
        if (GameController.Main.paused)
        {
            document.rootVisualElement.style.display = DisplayStyle.Flex;
            minimap.SetActive(false);

        }
        else
        {
            document.rootVisualElement.style.display = DisplayStyle.None;
            minimap.SetActive(true);
        }
    }
}
