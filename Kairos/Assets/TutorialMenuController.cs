using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialMenuController : MonoBehaviour
{
    public Button nextButton;
    public Button backButton;
    public Label tutorialText;
    public int currentText = 0;
    public string[] tutorials = { "Use 'WASD' to move around", "Use 'E' to move Up and 'Q' to move down" };

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
        currentText++;
        tutorialText.text = tutorials[currentText];
        if (currentText > tutorials.Length)
        {
            currentText = 0;
        }
    }
    
    void BackButtonPressed()
    {
        currentText--;
        tutorialText.text = tutorials[currentText];
    }
}
