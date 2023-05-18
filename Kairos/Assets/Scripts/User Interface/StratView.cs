using System.Collections;
using System.Collections.Generic;
using TMPro;
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
            GameController.Main.UIController.gameUI.destroyBuildingButton.visible = false;
            //BuildMenu.SetActive(false);
        }
        else
        {
            inspectee = gameObject;
            unitView.text = inspectee.name;
            if (gameObject.GetComponent<Structure>() != null && !gameObject.GetComponent<Selectable>().faction)
            {
                GameController.Main.UIController.gameUI.destroyBuildingButton.visible = true;
            }
        }
    }
    public void BuilderSelected()
    {
        GameController.Main.UIController.EnableBuildMenu(true);
        //BuildMenu.SetActive(!BuildMenu.active);
    }
}
