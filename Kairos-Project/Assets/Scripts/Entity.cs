using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class from which all units and structures inherit
/// </summary>
public abstract class Entity : MonoBehaviour
{
    [SerializeField]
    GameObject selectedHighlight;
    public bool select = false;

    public void SetSelectedVisible(bool selected)
    {
        if(selectedHighlight != null)
        {
            selectedHighlight.SetActive(selected);
            select = selected;
        }
    }

    protected void Start()
    {
        //!DELETE
        GameController.main.playerController.playerEntities.Add(gameObject);
    }

    private void OnDestroy()
    {
        GameController.main.playerController.playerEntities.Remove(gameObject);
    }
}
