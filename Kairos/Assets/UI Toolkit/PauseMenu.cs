using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    UIDocument document;
    public Label pauseLabel;
    public Label confirmationLabel;
    public Button exitButton;
    public Button confirmationExitButton;
    public Button backButton;
    public Button startButton;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        pauseLabel = document.rootVisualElement.Q<Label>("PauseLabel");
        confirmationLabel = document.rootVisualElement.Q<Label>("ConfirmationLabel");

        exitButton = document.rootVisualElement.Q<Button>("ExitButton");
        exitButton.RegisterCallback<ClickEvent>(ExitToConfirmationScreen);

        confirmationExitButton = document.rootVisualElement.Q<Button>("ConfirmExitButton");
        confirmationExitButton.RegisterCallback<ClickEvent>(ExitToMainMenu);

        backButton = document.rootVisualElement.Q<Button>("BackButton");
        backButton.RegisterCallback<ClickEvent>(BackToPause);

        //startButton = document.rootVisualElement.Q<Button>("StartButton");
        //startButton.RegisterCallback<ClickEvent>(StartGame);
    }

    public void ExitToConfirmationScreen(ClickEvent click)
    {
        pauseLabel.style.display = DisplayStyle.None;
        exitButton.style.display = DisplayStyle.None;

        confirmationLabel.style.display = DisplayStyle.Flex;
        confirmationExitButton.style.display = DisplayStyle.Flex;
        backButton.style.display = DisplayStyle.Flex;
    }

    public void ExitToMainMenu(ClickEvent click)
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void BackToPause(ClickEvent click)
    {
        pauseLabel.style.display = DisplayStyle.Flex;
        exitButton.style.display = DisplayStyle.Flex;

        confirmationLabel.style.display = DisplayStyle.None;
        confirmationExitButton.style.display = DisplayStyle.None;
        backButton.style.display = DisplayStyle.None;
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

        }
        else
        {
            document.rootVisualElement.style.display = DisplayStyle.None;
        }
    }
}
