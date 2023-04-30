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

    public Button strongholdButton;

    public Button barracksButton;

    public Button archerTowerButton;

    public Button resourceButton;

  

    public BuildMenu(VisualElement element)
    {
        Init(element);
    }

    public void Init(VisualElement element)
    {
        mainElement = element;

        strongholdButton = element.Q("StrongholdButton") as Button;
        strongholdButton.RegisterCallback<ClickEvent>(StrongholdButton_OnClick);

        barracksButton = element.Q("BarracksButton") as Button;
        barracksButton.RegisterCallback<ClickEvent>(Barracks_Button_OnClick);

        archerTowerButton = element.Q("ArcherTowerButton") as Button;
        archerTowerButton.RegisterCallback<ClickEvent>(ArcherTowerButton_OnClick);

        resourceButton = element.Q("ResourceButton") as Button;
        resourceButton.RegisterCallback<ClickEvent>(ResourceButton_OnClick);
    }

    public void EnableBuildMenu(bool enable)
    {
        mainElement.visible = enable;
        strongholdButton.visible = enable;
        barracksButton.visible = enable;
        archerTowerButton.visible = enable;
        resourceButton.visible = enable;
        mainElement.SetEnabled(enable);
    }

    private void StrongholdButton_OnClick(ClickEvent cl)
    {
        GameController.Main.StructureController.BuildOrder("stronghold");
    }
    private void Barracks_Button_OnClick(ClickEvent cl)
    {
        GameController.Main.StructureController.BuildOrder("barracks");
    }
    private void ArcherTowerButton_OnClick(ClickEvent cl)
    {
        GameController.Main.StructureController.BuildOrder("archertower");
    }
    private void ResourceButton_OnClick(ClickEvent cl)
    {
        GameController.Main.StructureController.BuildOrder("resource");
    }
}

public class ProductionMenu 
{
    public VisualElement mainElement;

    public Button infantryButton;

    public Button archerButton;

    public Button collectorButton;

    public Button builderButton;

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

        collectorButton = element.Q("RCButton") as Button;
        collectorButton.RegisterCallback<ClickEvent>(RCButton_OnClick);

        builderButton = element.Q("BuilderButton") as Button;
        builderButton.RegisterCallback<ClickEvent>(BuilderButton_OnClick);

        archerButton.visible = false;
        infantryButton.visible = false;
        collectorButton.visible = false;
        builderButton.visible = false;
    }

    public void EnableProductionMenu(bool enable)
    {
        mainElement.visible = enable;
        infantryButton.visible = enable;
        archerButton.visible = enable;
        collectorButton.visible = enable;
        builderButton.visible = enable;
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
    private void RCButton_OnClick(ClickEvent cl)
    {
        GameController.Main.StructureController.TrainCollector();
    }
    private void BuilderButton_OnClick(ClickEvent cl)
    {
        GameController.Main.StructureController.TrainBuilder();
    }
}
