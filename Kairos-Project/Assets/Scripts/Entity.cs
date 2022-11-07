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
    public KeyCode hoykey;
    public float rotateSpeed = 10f;

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

    protected void RotateTowards(Vector3 pos)
    {
        /*
        if (lockHorizontalRotation)
        {
            pos.y = transform.position.y;
        }*/

        pos.y = transform.position.y;
        Quaternion rotation = transform.rotation;

        Vector3 direction = pos - transform.position;
        var lookRotation = Quaternion.LookRotation(direction);


        rotation = Quaternion.Slerp(rotation, lookRotation, Time.deltaTime * rotateSpeed);
        transform.rotation = rotation;
    }
}
