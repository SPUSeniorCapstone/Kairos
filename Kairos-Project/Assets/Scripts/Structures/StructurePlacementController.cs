using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// credit to this: https://weeklyhow.com/rts-building-system-with-raycast/

/// <summary>
/// This class contains our Building System for our Structures in Kairos.
/// </summary>
public class StructurePlacementController : MonoBehaviour
{
    [SerializeField]
    private GameObject _placeholderBuilding;

    private GameObject _placeholder;

    [SerializeField]
    private Grid _grid;

    [SerializeField]
    private GameObject _building;

    private Vector3 _mousePosition;
    private float _previousX;
    private float _previousY;
    private float _previousZ;

    /// <summary>
    /// This function allows the player to place a Structure onto a Grid below the Terrain object
    /// </summary>
    void Update()
    {
        if (Input.GetKey(KeyCode.B))
        {
            HandleBuidlingPlacement();
        }else if (_placeholder != null)
        {
            Destroy(_placeholder.gameObject);
            _placeholder = null;
        }
    }

    void HandleBuidlingPlacement()
    {
        if(_building == null)
        {
            Debug.Log("No Building Given");
            return;
        }
        else if (_placeholderBuilding == null)
        {
            _placeholderBuilding = _building;
        }

        if (_placeholder == null)
        {
            // instantiate the building
            _placeholder = Instantiate(_placeholderBuilding);
        }

        // get the current mouse position
        _mousePosition = Input.mousePosition;

        // declare a raycast to get the mouse's position
        Ray ray = Camera.main.ScreenPointToRay(_mousePosition);
        RaycastHit hit;

        // if the raycast is present, start placing an structure on the grid
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            float positionX = hit.point.x;
            float positionZ = hit.point.z;
            float positionY = Terrain.activeTerrain.SampleHeight(new Vector3(positionX, 0, positionZ));

            // declare the vector that will determine the placement of the structure on the grid
            Vector3 newPos = _grid.GetCellCenterWorld(_grid.WorldToCell(new Vector3(positionX, positionY, positionZ)));

            // if there is no overlap between the previous position and current position, get the new position
            if (_previousX != positionX || _previousZ != positionZ)
            {
                _previousX = newPos.x;
                _previousY = newPos.y;
                _previousZ = newPos.z;

                //_placeholder.transform.position = new Vector3(positionX, 0f, positionZ);

                // place the structure to the new position vector
                _placeholder.transform.position = newPos;

                //Debug.Log(_placeholder.transform.position);
                //Debug.Log(_previousX + " / "+ _previousZ);
            }

            // use the left mouse button to place any structure onto the scene
            if (Input.GetMouseButtonUp(0))
            {
                Instantiate(_building, _placeholder.transform.position, Quaternion.identity, transform);
            }
        }
    }
}
