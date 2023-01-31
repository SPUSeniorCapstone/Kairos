using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public bool selected;
    public Material unSelectedMaterial;
    public Material selectedMaterial;
    void Start()
    {
        // write function to handles this or modify public list directly?
        GameController.Main.SelectionController.masterSelect.Add(this);
    }

    // test code delete when done
    public void Activate()
    {
        GetComponentInChildren<MeshRenderer>().material = selectedMaterial;
    }
    public void Deactivate()
    {
        GetComponentInChildren<MeshRenderer>().material = unSelectedMaterial;
    }

}
