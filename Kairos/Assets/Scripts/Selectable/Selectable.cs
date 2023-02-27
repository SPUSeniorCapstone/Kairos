using UnityEngine;

public class Selectable : MonoBehaviour
{
    public bool selected;
    public bool faction;
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
}
