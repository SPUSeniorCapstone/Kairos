using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// credit to this: https://weeklyhow.com/rts-building-system-with-raycast/

public class BuildingSystem : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        // instantiate the building
        _placeholder = Instantiate(_placeholderBuilding);
    }

    // Update is called once per frame
    void Update()
    {
        // get the current mouse position
        _mousePosition = Input.mousePosition;

        // declare a raycast to get the mouse's position
        Ray ray = Camera.main.ScreenPointToRay(_mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            float positionX = hit.point.x;
            float positionZ = hit.point.z;
            float positionY = Terrain.activeTerrain.SampleHeight(new Vector3(positionX, 0, positionZ));
            Vector3 newPos = _grid.GetCellCenterWorld(_grid.WorldToCell(new Vector3(positionX, positionY, positionZ)));

            if (_previousX != positionX || _previousZ != positionZ)
            {
                _previousX = newPos.x;
                _previousY = newPos.y;
                _previousZ = newPos.z;

                //_placeholder.transform.position = new Vector3(positionX, 0f, positionZ);
                _placeholder.transform.position = newPos;

                Debug.Log(_placeholder.transform.position);
                //Debug.Log(_previousX + " / "+ _previousZ);
            }

            // use the left mouse button to place any structure onto the scene
            if (Input.GetMouseButtonUp(0))
            {
                Instantiate(_building, _placeholder.transform.position, Quaternion.identity);
            }

        }
    }
}
