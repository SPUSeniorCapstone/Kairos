using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public bool selected;
    void Start()
    {
        // write function to handles this or modify public list directly?
        GameController.main.selectionController.masterSelect.Add(this);
    }

}
