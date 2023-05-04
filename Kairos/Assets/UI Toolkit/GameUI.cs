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

    public Label ResourceCounter;

    public Label NodeCounter;

    public Button destroyBuildingButton;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
        ResourceCounter = document.rootVisualElement.Q("numberResource") as Label;
        NodeCounter = document.rootVisualElement.Q("numberNodes") as Label;
        destroyBuildingButton = document.rootVisualElement.Q("DestroyButton") as Button;
        destroyBuildingButton.visible = false;
     

     
        ResourceCounter.text = FormatNum(GameController.Main.resouceCount, true);
        NodeCounter.text = FormatNum(GameController.Main.StructureController.CorruptionNodes.Count, false);
    }

    private void OnEnable()
    {
        if (document != null)
        BuildMenu = new BuildMenu(document.rootVisualElement.Q("BuildMenu"));
        ProductionMenu = new ProductionMenu(document.rootVisualElement.Q("ProductionMenu"));
    }

    public string FormatNum(int num, bool greater)
    {
        if (num < 10 && greater)
        {
            return "000" + num.ToString();
        }
        else if (num < 100 && greater)
        {
            return "00" + num.ToString();
        }
        else if ((num < 1000 && greater ) || (num < 10 && !greater))
        {
            return "0" + num.ToString();
        }
        else
        {
            return num.ToString();
        }
    }

    public void UpdateResource(int count)
    {
        ResourceCounter.text = FormatNum(count, true);
    }

    public void UpdateNodes(int count)
    {
        NodeCounter.text = FormatNum(count, false);
    }

    public void OnStruture(bool enable)
    {
        destroyBuildingButton.visible = enable;
    }
}



public class BuildMenu
{

    public VisualElement mainElement;

    public Button strongholdButton;

    public Button barracksButton;

    public Button archerTowerButton;

    public Button purifierButton;

  

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

        purifierButton = element.Q("PurifierButton") as Button;
        purifierButton.RegisterCallback<ClickEvent>(ResourceButton_OnClick);
    }

    public void EnableBuildMenu(bool enable)
    {
        mainElement.visible = enable;
        strongholdButton.visible = enable;
        barracksButton.visible = enable;
        archerTowerButton.visible = enable;
        purifierButton.visible = enable;
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
        GameController.Main.StructureController.BuildOrder("purifier");
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
        if (cl.button == 0)
        {
            GameController.Main.StructureController.TrainInfantry();
        } 
        else if (cl.button == 1)
        {
            Debug.Log("Right click to cancel");
        }
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
