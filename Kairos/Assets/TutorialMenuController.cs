using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class TutorialMenuController : MonoBehaviour
{
    public Button nextButton;
    public Button backButton;
    public Button hideTutorialButton;
    public Button showTutorialButton;
    public Label tutorialText;
    private int currentText = 0;
    private string[] tutorials = {"Use 'WASD' to move around", "Use 'E' to move Up and 'Q' to move down", "Open up the Build Menu by 'Left Clicking' the Builder Unit", "Watch out! The corruption is speading.\nBuild a Purifier near the corruption to begin purifying the land", 
        "Build a Barracks from the Build Menu", "Open the Barracks Menu by 'Left Clicking' the Barracks", "Train a unit by 'Left Clicking' the infantry button", "Queue 5 units by holding 'Shift' and 'Left Clicking' one of the unit buttons", "To select multiple units hold 'Left Click' and drag your mouse", 
        "When you have multiple units selected, 'Right Click' on a point where you want the units to go", "Keep in mind units will take damage when standing in the corruption", "Send out units to defeat the corrupted nodes and achieve Victory"};

    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        nextButton = root.Q<Button>("Next");
        backButton = root.Q<Button>("Back");
        tutorialText = root.Q<Label>("Tutorial-Text");
        hideTutorialButton = root.Q<Button>("Hide-Tutorial");
        showTutorialButton = root.Q<Button>("Show-Tutorial");

        nextButton.clicked += NextButtonPressed;
        backButton.clicked += BackButtonPressed;
        hideTutorialButton.clicked += HideTutorialPressed;
        showTutorialButton.clicked += ShowTutorialPressed;

        if (GameController.Main.paused)
        {
            nextButton.style.display = DisplayStyle.None;
            backButton.style.display = DisplayStyle.None;
            tutorialText.style.display = DisplayStyle.None;
            hideTutorialButton.style.display = DisplayStyle.None;
            showTutorialButton.style.display = DisplayStyle.None;
        }
    }
    void NextButtonPressed()
    {
        currentText++;
        if (currentText == tutorials.Length)
        {
            hideTutorialButton.style.display = DisplayStyle.Flex;
            currentText = 0;
        }
        tutorialText.text = tutorials[currentText];
    }
    
    void BackButtonPressed()
    {
        currentText--;
        if (currentText < 0)
        {
            currentText = 0;
        }
        tutorialText.text = tutorials[currentText];
    }
    public void StartGamePressed()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
    public void HideTutorialPressed()
    {
        nextButton.style.display = DisplayStyle.None;
        backButton.style.display = DisplayStyle.None;
        tutorialText.style.display = DisplayStyle.None;
        hideTutorialButton.style.display = DisplayStyle.None;
        showTutorialButton.style.display = DisplayStyle.Flex;
    }
    public void ShowTutorialPressed()
    {
        nextButton.style.display = DisplayStyle.Flex;
        backButton.style.display = DisplayStyle.Flex;
        tutorialText.style.display = DisplayStyle.Flex;
        hideTutorialButton.style.display = DisplayStyle.Flex;
        showTutorialButton.style.display = DisplayStyle.None;
    }
}
