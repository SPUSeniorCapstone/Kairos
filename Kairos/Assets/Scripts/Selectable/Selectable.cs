using UnityEngine;

public class Selectable : MonoBehaviour
{
    public bool selected;
    public bool faction;
    protected Material unSelectedMaterial;
    protected Material selectedMaterial;
    private Unit unit;
    private Structure structure;
    public Shader unHighlight;

    public int clickCount;
    public float clickTime;
    public float clickDelay = .5f;
    public bool selectDoubleClick = false;
    public bool massSelected = false;

    void Start()
    {
        selectedMaterial = GetComponentInChildren<MeshRenderer>().material;
        unHighlight = GetComponentInChildren<MeshRenderer>().material.shader;
        // write function to handles this or modify public list directly?
        GameController.Main.SelectionController.masterSelect.Add(this);

        //unSelectedMaterial = gameObject.GetComponentInChildren<Material>();

        // this is so that when selected, selectable can fire any event that needs to happen in unit or structure
        if(GetComponent<Unit>() != null)
        {
            unit = GetComponent<Unit>();
        }
        else if (GetComponent<Structure>() != null)
        {
            structure = GetComponent<Structure>();
        }
    }
    public void Activate()
    {
       
        if (GameController.Main.SelectionController.testCooldown)
        {
            if (selected)
            {
                Highlight();
            }
            if (unit != null)
            {
                // && !massSelected
                if (selectDoubleClick)
                {
                    Debug.Log("DOUBLE CLICK!!!---" + clickCount);
                    unit.doubleClicked = true;
                    selectDoubleClick = false;
                    clickCount = 0;
                    clickTime = 0;
                }
                unit.OnSelect();
            }
            else if (structure != null)
            {
                //if (selectDoubleClick)
                //{
                //    unit.doubleClicked = true;
                //}
                structure.OnSelect();
            }
        }
    }
    public void Deactivate()
    {
        if (GameController.Main.SelectionController.testCooldown)
        {
            clickTime = 0;
            clickCount = 0;
            massSelected = false;
            UnHighlight();
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

    public void Highlight()
    {
        selectedMaterial.shader = GameController.Main.highlight;
        GetComponentInChildren<MeshRenderer>().material = selectedMaterial;
    }
    public void UnHighlight()
    {
        selectedMaterial.shader = unHighlight;
        GetComponentInChildren<MeshRenderer>().material = selectedMaterial;
    }

    // Next two functions do with hover over triggers

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
            GameController.Main.SelectionController.actionTarget = this.gameObject;

            //Debug.Log("True enemy, pos" + GameController.main.playerController.enemyPos);

            // set cursor enemy
            //Cursor.SetCursor(GameController.Main.enemy, Vector2.zero, CursorMode.Auto);
        }
        else
        {

            GameController.Main.SelectionController.actionTarget = this.gameObject;
            // set cursor other
            //Cursor.SetCursor(GameController.Main.capture, Vector2.zero, CursorMode.Auto);

            //Debug.Log("The mouse sees me");
            //Debug.Log(GameController.main.capture);
        }


    }
    private void OnMouseExit()
    {
        GameController.Main.SelectionController.onEnemy = false;
        GameController.Main.SelectionController.actionTarget = null;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        // Debug.Log("off");
    }

    public void OnClick()
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            massSelected = false;
            //!GameController.Main.SelectionController.currentlySelect.Contains(this)
            if (GameController.Main.SelectionController.AllowDoubleClicks && !faction)
            {
                clickCount++;
                if (clickCount == 1)
                {
                    clickTime = Time.time;
                }
                else if (clickCount > 1 && Time.time - clickTime < clickDelay)
                {
                    selectDoubleClick = true;
                }
                else
                {
                    clickTime = 0;
                    clickCount = 0;
                }
            }
            if (!selected)
            {
                GameController.Main.SelectionController.currentlySelect.Add(this);
            }
            GameController.Main.UIController.StratView.SetUnitView(gameObject);
            selected = true;
            Activate();
        }
    }

    private void OnMouseDown()
    {
        OnClick();
    }

    // master destroy should deal with this?
    //private void OnDestroy()
    //{
    //    GameController.Main.SelectionController.masterSelect.Remove(this);
    //    GameController.Main.SelectionController.currentlySelect.Remove(this);
    //}
}
