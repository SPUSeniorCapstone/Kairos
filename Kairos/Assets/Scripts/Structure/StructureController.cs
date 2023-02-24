using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class StructureController : MonoBehaviour
{
    public bool StructurePlacementMode = false;

    public Structure structure;
    public GameObject placeHolder;

    public ProductionStructure selected;

    private void Start()
    {
        //PlaceStructure(structure, FindObjectOfType<CreateWorld>().strongholdPos);
    }

    void Update()
    {
        if (StructurePlacementMode)
        {
            Vector3Int pos = Vector3Int.zero; 

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000, LayerMask.GetMask("Terrain")))
            {
                Vector3 point = hit.point;
                var w = GameController.Main.WorldController;
                pos = w.WorldToBlockPosition(point);
                pos.y = w.World.GetHeight(pos);
            }


            placeHolder.transform.position = pos;

            if (GameController.Main.inputController.Select.Down())
            {
                PlaceStructure(structure, pos);
            }
        }
    }

    public void TrainUnit()
    {
        selected.QueueUnits();
    }

    public void PlaceStructure(Structure structure, Vector3Int position)
    {
        if(!IsValidPlacement(structure, position))
        {
            Debug.Log("Invalid Structure Placement");
            return;
        }


        var w = GameController.Main.WorldController;

        position.y = w.World.GetHeight(position);

        var s = Instantiate<Structure>(structure);
        s.transform.position = position;

        for (int x = 0; x < structure.Size.x; x++)
        {
            for (int z = 0; z < structure.Size.z; z++)
            {
                int h = w.World.GetHeight(position.x + x, position.z + z);
                w.World.SetHeight(position.x + x, position.z + z, h + structure.Size.y);
            }
        }
    }

    public bool IsValidPlacement(Structure structure, Vector3Int position)
    {
        var w = GameController.Main.WorldController;
        int h = w.World.GetHeight(position);

        for (int x = 0; x < structure.Size.x; x++)
        {
            for (int z = 0; z < structure.Size.z; z++)
            {
                int h2 = w.World.GetHeight(position.x + x, position.z + z);
                if(h2 != h)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
