using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class GameUI : MonoBehaviour
{
    UIDocument document;

    public BuildMenu BuildMenu;

    public ProductionMenu ProductionMenu;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        if (document != null)
        BuildMenu = new BuildMenu(document.rootVisualElement.Q("BuildMenu"));
        ProductionMenu = new ProductionMenu(document.rootVisualElement.Q("ProductionMenu"));
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

public class ProductionMenu 
{
    public VisualElement mainElement;

    public Button infantryButton;

    public Button archerButton;

    public ProductionMenu(VisualElement element)
    {
        Init(element);
    }

    public void Init(VisualElement element)
    {
        mainElement = element;

        infantryButton = element.Q("InfantryButton") as Button;
        infantryButton.RegisterCallback<ClickEvent>(InfantryButton_OnClick);

        archerButton = element.Q("ArcherButton") as Button;
        archerButton.RegisterCallback<ClickEvent>(ArcherButton_OnClick);

        archerButton.visible = false;
        infantryButton.visible = false;
    }

    public void EnableProductionMenu(bool enable)
    {
        mainElement.visible = enable;
        infantryButton.visible = enable;
        archerButton.visible = enable;
        mainElement.SetEnabled(enable);
    }

    private void InfantryButton_OnClick(ClickEvent cl)
    {
        GameController.Main.StructureController.TrainInfantry();
    }

    private void ArcherButton_OnClick(ClickEvent cl)
    {
        GameController.Main.StructureController.TrainArcher();
    }
}
