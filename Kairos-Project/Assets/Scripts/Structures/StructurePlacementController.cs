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
    private GameObject placeholderBuildingPrefab;
    [SerializeField]
    private Structure buildingPrefab;

    private GameObject tempBuilding;


    private Vector3 _mousePosition;
    private Vector3 previousPosition;

    /// <summary>
    /// This function allows the player to place a Structure onto a Grid below the Terrain object
    /// </summary>
    void Update()
    {
        if (Input.GetKey(KeyCode.B))
        {
            HandleBuidlingPlacement();
        }
        else if (tempBuilding != null)
        {
            Destroy(tempBuilding.gameObject);
            tempBuilding = null;
        }
    }

    void HandleBuidlingPlacement()
    {

        if (tempBuilding == null)
        {
            // instantiate the building
            tempBuilding = Instantiate(placeholderBuildingPrefab);
        }

        // get the current mouse position
        _mousePosition = Input.mousePosition;

        // declare a raycast to get the mouse's position
        Ray ray = Camera.main.ScreenPointToRay(_mousePosition);
        RaycastHit hit;

        // if the raycast is present, start placing an structure on the grid
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            Vector3 mouseMapPos = new Vector3(hit.point.x, hit.point.y, hit.point.z);

            // declare the vector that will determine the placement of the structure on the grid
            Vector3Int newPos = MapController.main.grid.WorldToCell(new Vector3(hit.point.x, hit.point.z, 0));
            Vector2Int mapPos = new Vector2Int(newPos.x, newPos.z);

            Vector3 tempPos = MapController.main.grid.CellToWorld(new Vector3Int(mapPos.x, mapPos.y, 0));
            tempPos.y = MapController.main.RTS.SampleHeight(new Vector3(tempPos.x, 0, tempPos.z));

            // if there is no overlap between the previous position and current position, get the new position
            if (previousPosition.x != mouseMapPos.x || previousPosition.z != mouseMapPos.z)
            {
                previousPosition = tempPos;
                tempBuilding.transform.position = tempPos;
            }

            // use the left mouse button to place any structure onto the scene
            if (Input.GetMouseButtonUp(0))
            {
                PlaceBuilding(buildingPrefab, mapPos);
            }
        }
    }

    void PlaceBuilding(Structure building, Vector2Int mapPos)
    {
        Vector3 placePos = MapController.main.grid.CellToWorld(new Vector3Int(mapPos.x, mapPos.y, 0));

        MapController.main.mapData.MarkOffPassability(mapPos, building.width, building.length, false);
        StartCoroutine(DropBuilding(placePos, -5, 0.5f));



        IEnumerator DropBuilding(Vector3 placePosition, float h, float t)
        {
            Vector3 p = placePosition;
            p.y += h;
            Structure b = Instantiate(buildingPrefab, p, Quaternion.identity, transform);
            float terrainHeight = MapController.main.RTS.SampleHeight(p);

            float stopTime = Time.time + t;
            while (stopTime > Time.time)
            {
                float le = Mathf.Clamp((stopTime - Time.time) / t, 0, 1);
                b.transform.position = Vector3.Lerp(placePosition, p, le);
                yield return null;
            }
            b.transform.position = placePosition;
        }
    }
}
