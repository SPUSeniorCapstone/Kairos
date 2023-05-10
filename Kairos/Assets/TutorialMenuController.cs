using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialMenuController : MonoBehaviour
{
    public Button nextButton;
    public Button backButton;
    public Label tutorialText;

    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        nextButton = root.Q<Button>("Next");
        backButton = root.Q<Button>("Back");
        tutorialText = root.Q<Label>("Tutorial-Text");

        nextButton.clicked += NextButtonPressed;
        backButton.clicked += BackButtonPressed;
    }

    void NextButtonPressed()
    {
        tutorialText.text = "Use 'WASD' to move the camera";
        tutorialText.style.display = DisplayStyle.Flex;
    }
    
    void BackButtonPressed()
    {
        tutorialText.text = "Use 'WASD' to move the camera";
        tutorialText.style.display = DisplayStyle.None;
    }
}
