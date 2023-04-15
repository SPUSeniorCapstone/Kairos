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
        BuildMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUnitView(GameObject gameObject)
    {
        if (gameObject == null)
        {
            inspectee = null;
            unitView.text = "New Text";
            BuildMenu.SetActive(false);
        }
        else
        {
            inspectee = gameObject;
            unitView.text = inspectee.name;
        }
    }
    public void BuilderSelected()
    {
        BuildMenu.SetActive(!BuildMenu.active);
    }
}
