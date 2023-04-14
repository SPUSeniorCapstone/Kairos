using UnityEngine;

public class Selectable : MonoBehaviour
{
    public bool selected;
    public bool faction;
    public Material unSelectedMaterial;
    public Material selectedMaterial;
    private Unit unit;
    private Structure structure;
   
    void Start()
    {
        // write function to handles this or modify public list directly?
        GameController.Main.SelectionController.masterSelect.Add(this);

        if(GetComponent<Unit>() != null)
        {
            unit = GetComponent<Unit>();
        }
        else if (GetComponent<Structure>() != null)
        {
            structure = GetComponent<Structure>();
        }
    }

    // test code delete when done
    public void Activate()
    {
       if (GameController.Main.SelectionController.testCooldown)
        {
            GetComponentInChildren<MeshRenderer>().material = selectedMaterial;
            if (unit != null)
            {
                unit.OnSelect();
            }
            else if (structure != null)
            {
                structure.OnSelect();
            }
        }
    }
    public void Deactivate()
    {
        if (GameController.Main.SelectionController.testCooldown)
        {
            GetComponentInChildren<MeshRenderer>().material = unSelectedMaterial;
            if (unit != null)
            {
                unit.OnDeselect();
            }
            else if (structure != null)
            {
                structure.OnDeselect();
            }
        }
    }
    private void OnMouseOver()
    {
        //if (GameController.Main.capture == null)
        //{
        //    Debug.Log("Unsuccesful");
        //}
        if (faction)
        {
            Debug.Log("Enimico!");
            GameController.Main.SelectionController.onEnemy = true;
            GameController.Main.SelectionController.enemy = this.gameObject;

            //Debug.Log("True enemy, pos" + GameController.main.playerController.enemyPos);

            // set cursor enemy
            //Cursor.SetCursor(GameController.Main.enemy, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            // set cursor other
            //Cursor.SetCursor(GameController.Main.capture, Vector2.zero, CursorMode.Auto);

            //Debug.Log("The mouse sees me");
            //Debug.Log(GameController.main.capture);
        }


    }
    private void OnMouseExit()
    {
        GameController.Main.SelectionController.onEnemy = false;
        GameController.Main.SelectionController.enemy = null;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        // Debug.Log("off");
    }

    // master destroy should deal with this?
    //private void OnDestroy()
    //{
    //    GameController.Main.SelectionController.masterSelect.Remove(this);
    //    GameController.Main.SelectionController.currentlySelect.Remove(this);
    //}
}
