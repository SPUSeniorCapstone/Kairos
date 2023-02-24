using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using static UnityEditor.Progress;

public class SelectionController : MonoBehaviour
{
    public bool onEnemy;
    public GameObject enemy;
    private Vector3 startPosition;
    public List<Selectable> masterSelect = new List<Selectable>();
    public List<Selectable> currentlySelect = new List<Selectable>();
    // is this neccessary

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
            if (GameController.Main.InputController.Select.KeyUp())
            {
                Debug.Log("Mouse0 up");
                selectionAreaTransform.gameObject.SetActive(false);
                foreach(Selectable selectable in masterSelect)
                {
                    selectable.selected = false;
                    selectable.Deactivate();
                    currentlySelect.Remove(selectable);
                    var point = Camera.main.WorldToScreenPoint(selectable.transform.position);
                    // will this work? will it remember these "global" vectors?
                    if (point.x > lowerLeft.x && point.x < upperRight.x && point.y > lowerLeft.y && point.y < upperRight.y && !selectable.faction)
                    {
                        //Debug.Log("Within bounds");
                        selectable.selected = true;
                        selectable.Activate();
                        currentlySelect.Add(selectable);
                    }
                    // this will check if the mouse click ray hit a selectable
                    else
                    {
                        Selectable oneClick = GetMouseWorldPosition3D();
                        if (oneClick != null && !oneClick.selected && !oneClick.faction)
                        {
                            oneClick.selected = true;
                            oneClick.Activate();
                            currentlySelect.Add(oneClick);
                            Debug.Log(oneClick);
                        }
                    }
                }
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
                lowerLeft =  new Vector3(
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
    // does this need to run in update?
    // don't know if get componet in parent will work now
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
        else if (hitData.transform != null) { Debug.Log(hitData.transform.name); }
        return null;
    }
}
