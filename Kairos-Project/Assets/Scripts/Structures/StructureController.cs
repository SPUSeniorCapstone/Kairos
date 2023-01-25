using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureController : MonoBehaviour
{
    public GameObject structureMenuUI;
    
    public void OnMouseDown()
    {
        structureMenuUI.SetActive(true);
    }
}
