using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public bool onEnemy;
    // originally enemy, but i think should be generalized
    public GameObject actionTarget;
    private Vector3 startPosition;
    public List<Selectable> masterSelect = new List<Selectable>();
    public List<Selectable> currentlySelect = new List<Selectable>();

    // this only exists to try and fix clicking on something that just got instantiated (trying to prevent it, works only sometimes)
    public bool testCooldown = true;

    [SerializeField] private GameObject selectionAreaTransform;

    Vector3 lowerLeft;
    Vector3 upperRight;
    Vector3 lowerRight;
    Vector3 upperLeft;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.Main.paused)
        {
            // on mouse 2, attack enemy or path find to location
            //if (GameController.Main.InputController.Command.Down())
            //{
            //    Debug.Log("Mouse1 down");
            //    if (onEnemy)
            //    {
            //        //-----------------------------
            //    }
            //    else
            //    {
            //        GameController.Main.CommandController.MoveSelected(currentlySelect);
            //    }
            //}
            // single click select, or click and drag let go
            if (GameController.Main.InputController.Select.KeyUp() && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                selectionAreaTransform.gameObject.SetActive(false);
                bool kontinue = true;
                foreach (Selectable selectable in masterSelect)
                {
                    var point = Camera.main.WorldToScreenPoint(selectable.transform.position);
                    if (kontinue == true && point.x > lowerLeft.x && point.x < upperRight.x && point.y > lowerLeft.y && point.y < upperRight.y && !selectable.faction)
                    {
                        if (selectable.selected == false)
                        {
                            currentlySelect.Add(selectable);
                            
                            // what to add so this only fires if the current unit view isn't part of currentlyselect after new selection occurs?
                            GameController.Main.UIController.StratView.SetUnitView(selectable.gameObject);
                            
                        }
                        selectable.selected = true;
                        selectable.Activate();
                        kontinue = false;
                    }
                    // this will check if the mouse click ray hit a selectable (uses oneclick instead of selectable)
                    // does this work as intended? what if you flick the mouse?
                    else if (kontinue == true)
                    {
                        Selectable oneClick = GetMouseWorldPosition3D();
                        if (oneClick != null && !oneClick.selected && !oneClick.faction)
                        {
                            if (oneClick.selected == false)
                            {
                                currentlySelect.Add(oneClick);
                            }
                            GameController.Main.UIController.StratView.SetUnitView(oneClick.gameObject);
                            oneClick.selected = true;
                            oneClick.Activate();
                            Debug.Log(oneClick);
                            kontinue = false;
                        }
                    }
                    if (selectable.selected == true && kontinue == true)
                    {
                        Debug.Log(selectable + " deactivate");
                        selectable.selected = false;
                        selectable.Deactivate();
                        currentlySelect.Remove(selectable);
                    }
                }
                // ping ui to check
                if (currentlySelect.Count > 0) { }
                    //GameController.Main.UIController.StratView.SetUnitView(currentlySelect[0].gameObject);
                else GameController.Main.UIController.StratView.SetUnitView(null);
            }
            // click and drag box
            if (GameController.Main.InputController.Select.Pressed())
            {
                if (GameController.Main.InputController.Select.Down())
                {
                    startPosition = Input.mousePosition;
                    selectionAreaTransform.gameObject.SetActive(true);
                }
                Vector3 currentMousePosition = Input.mousePosition;
                float z = currentMousePosition.z;
                lowerLeft = new Vector3(
                        Mathf.Min(startPosition.x, currentMousePosition.x),
                        Mathf.Min(startPosition.y, currentMousePosition.y),
                        z
                );
                upperRight = new Vector3(
                    Mathf.Max(startPosition.x, currentMousePosition.x),
                     Mathf.Max(startPosition.y, currentMousePosition.y),
                     z
                   );
                // shouldn't matter: just the lower left and upper ight corner shouls matter right?
                lowerRight = new Vector3(
                   upperRight.x,
                   lowerLeft.y,
                   z
                  );
                upperLeft = new Vector3(
                   lowerLeft.x,
                   upperRight.y,
                   z
                  );

                //================================================================================================================

                var line = selectionAreaTransform.GetComponent<LineRenderer>();
                line.positionCount = 5;
                List<Vector3> vector3s = new List<Vector3>();
                vector3s.Add(MouseToWorld(upperLeft));
                vector3s.Add(MouseToWorld(upperRight));
                vector3s.Add(MouseToWorld(lowerRight));
                vector3s.Add(MouseToWorld(lowerLeft));
                vector3s.Add(MouseToWorld(upperLeft));
                line.SetPositions(vector3s.ToArray());
            }
        }
    }
    // should this be in a helper class?
    // returns a screen to world point with a constant vector.z
    private Vector3 MouseToWorld(Vector3 vector)
    {
        vector.z = 2.5f;
        return Camera.main.ScreenToWorldPoint(vector);
    }
    private Selectable GetMouseWorldPosition3D()
    {
        Ray ray;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData, 1000) && (hitData.transform.GetComponentInParent<Selectable>() || hitData.transform.GetComponent<Selectable>())) // <- will this be problematic?
        {
            Debug.Log(hitData.transform.name);
            return hitData.transform.GetComponentInParent<Selectable>();
        }
        else if (hitData.transform != null) { //Debug.Log(hitData.transform.name);
                                              }
        return null;
    }
}
