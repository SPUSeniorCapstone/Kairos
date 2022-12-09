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
    public KeyCode hotkey;
    public float rotateSpeed = 10f;
    public bool isEnemy;
   

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

    private void OnMouseOver()
    {
        if (GameController.main.capture == null)
        {
            Debug.Log("Unsuccesful");
        }
        if (isEnemy)
        {
            GameController.main.playerController.onEnemy = true;
            Debug.Log("True enemy");
        }
        Cursor.SetCursor(GameController.main.capture, Vector2.zero, CursorMode.Auto);
        Debug.Log("The mouse sees me");
        Debug.Log(GameController.main.capture);
      
    }
    private void OnMouseExit()
    {
        GameController.main.playerController.onEnemy = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Debug.Log("off");
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
