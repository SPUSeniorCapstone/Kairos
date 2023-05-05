using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    UIDocument document;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        var exitButton = document.rootVisualElement.Q("ExitButton") as Button;
        exitButton.RegisterCallback<ClickEvent>(ExitToMainMenu);

    }

    public void ExitToMainMenu(ClickEvent click)
    {
        SceneManager.LoadScene("MainMenu");
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
