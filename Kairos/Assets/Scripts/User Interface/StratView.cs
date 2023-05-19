using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StratView : MonoBehaviour
{
    public GameObject inspectee;
    public TextMeshProUGUI unitView;
    public GameObject BuildMenu;
    // Start is called before the first frame update
    void Start()
    {
        GameController.Main.UIController.EnableBuildMenu(false);
    }

    public void SetUnitView(GameObject gameObject)
    {
        if (gameObject == null)
        {
            inspectee = null;
            unitView.text = "New Text";
            GameController.Main.UIController.EnableBuildMenu(false);
            GameController.Main.UIController.gameUI.ProductionMenu.EnableProductionMenu(false);
            GameController.Main.UIController.gameUI.destroyBuildingButton.visible = false;
            //BuildMenu.SetActive(false);
        }
        else
        {
            inspectee = gameObject;
            unitView.text = inspectee.name;
            if (gameObject.GetComponent<Structure>() != null && !gameObject.GetComponent<Selectable>().faction)
            {
                if (gameObject.GetComponent<Stronghold>() == null || (gameObject.GetComponent<Stronghold>() != null && FindObjectsByType<Stronghold>(FindObjectsSortMode.None).Length > 1))
                { 
                    GameController.Main.UIController.gameUI.destroyBuildingButton.visible = true;
                }
                if (gameObject.GetComponent<ProductionStructure>())
                {
                    GameController.Main.UIController.EnableProductionMenu(true);
                    gameObject.GetComponent<ProductionStructure>().SetProdUI();
                }
                    GameController.Main.UIController.gameUI.BuildMenu.EnableBuildMenu(false);
            } 
            else if (gameObject.GetComponent<Unit>() != null && !gameObject.GetComponent<Selectable>().faction)
            {
                GameController.Main.UIController.gameUI.destroyBuildingButton.visible = false;
                GameController.Main.UIController.gameUI.ProductionMenu.EnableProductionMenu(false);
                if (gameObject.GetComponent<Builder_Unit>() == null)
                {
                    GameController.Main.UIController.gameUI.BuildMenu.EnableBuildMenu(false);
                }
            }
        }
    }
    public void BuilderSelected()
    {
        GameController.Main.UIController.EnableBuildMenu(true);
        //BuildMenu.SetActive(!BuildMenu.active);
    }
}
