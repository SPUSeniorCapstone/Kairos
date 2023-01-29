using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public bool onEnemy;
    private Vector3 startPosition;
    // is this neccessary

    [SerializeField] private GameObject selectionAreaTransform;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.main.paused)
        {
            // on mouse 2, attack enemy or path find to location
            if (GameController.main.inputController.Command.Down())
            {
                Debug.Log("Mouse1 down");
                if (onEnemy)
                {
                    //-----------------------------
                }
                else
                {
                    //MoveSelected();
                }
            }
            // single click select
            if (GameController.main.inputController.Select.KeyUp())
            {
                Debug.Log("Mouse0 up");
                selectionAreaTransform.gameObject.SetActive(false);
            }
            // click and drag box
            if (GameController.main.inputController.Select.Pressed())
            {
                if (GameController.main.inputController.Select.Down())
                {
                    //=========================================================
                    Debug.Log("Mouse0 down");
                    startPosition = Input.mousePosition;
                    selectionAreaTransform.gameObject.SetActive(true);
                }
                Debug.Log("Mouse0 pressed");
                // while mouse held down
                Vector3 currentMousePosition = Input.mousePosition;
                float z = currentMousePosition.z;
                Vector3 lowerLeft = new Vector3(
                        Mathf.Min(startPosition.x, currentMousePosition.x),
                        Mathf.Min(startPosition.y, currentMousePosition.y),
                        z
                );
                Vector3 upperRight = new Vector3(
                     Mathf.Max(startPosition.x, currentMousePosition.x),
                      Mathf.Max(startPosition.y, currentMousePosition.y),
                      z
                    );
                // shouldn't matter
                Vector3 lowerRight = new Vector3(
                    upperRight.x,
                    lowerLeft.y,
                    z
                   );
                Vector3 upperLeft = new Vector3(
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
    // should this bee in a helper class?
    private Vector3 MouseToWorld(Vector3 vector)
    {
        vector.z = 2.5f;
        return Camera.main.ScreenToWorldPoint(vector);
    }
}
