using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class GameUI : MonoBehaviour
{
    UIDocument document;

    public BuildMenu BuildMenu;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        if (document != null)
        BuildMenu = new BuildMenu(document.rootVisualElement.Q("BuildMenu"));
    }


}

public class BuildMenu
{

    public VisualElement mainElement;

    public Button buildButton;

    public BuildMenu(VisualElement element)
    {
        Init(element);
    }

    public void Init(VisualElement element)
    {
        mainElement = element;

        buildButton = element.Q("BuildButton") as Button;
        buildButton.RegisterCallback<ClickEvent>(BuildButton_OnClick);
    }

    public void EnableBuildMenu(bool enable)
    {
        mainElement.visible = enable;
        buildButton.visible = enable;
        mainElement.SetEnabled(enable);
    }

    private void BuildButton_OnClick(ClickEvent cl)
    {
        GameController.Main.StructureController.BuildOrder();
    }

}