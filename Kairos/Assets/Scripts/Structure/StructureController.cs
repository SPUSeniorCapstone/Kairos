using System.Collections.Generic;
using UnityEngine;

public class StructureController : MonoBehaviour
{
    public bool StructurePlacementMode = false;

    public List<Structure> masterStructure;

    // don't know how else to do this
    public GameObject PlayerStructures;
    public GameObject EnemyStructures;

    

    public Structure StructureToSpawn;
    public GameObject structurePreview;

    public Structure corruptionNode;

    public ProductionStructure selected;

    // PLEASE FIX THIS
    public GameObject infantry;
    public GameObject archer;

    private void Start()
    {
        //PlaceStructure(strongHold, GameController.Main.WorldController.WorldGenerator.strongholdPos);
        foreach (var pos in GameController.Main.WorldController.WorldGenerator.corruptionNodePositions)
        {
            PlaceStructure(corruptionNode, pos);
        }
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


            structurePreview.transform.position = pos;

            if (GameController.Main.inputController.Select.Pressed())
            {
                StructurePlacementMode = false;
                GameController.Main.SelectionController.testCooldown = false;
                GameController.Main.UIController.StratView.inspectee.GetComponent<Builder_Unit>().BuildTask(pos);
                PlaceStructure(StructureToSpawn, pos);
                structurePreview.transform.position = Vector3.zero;
                GameController.Main.SelectionController.testCooldown = true;
            }
        }
    }

    public void BuildOrder()
    {
        StructurePlacementMode = true;
        
    }

    public void TrainArcher()
    {
        selected.QueueUnits(archer);
    }
    public void TrainInfantry()
    {
        selected.QueueUnits(infantry);
    }

    public void PlaceStructure(Structure structure, Vector3Int position)
    {
        if (!IsValidPlacement(structure, position))
        {
            Debug.Log("Invalid Structure Placement");
            return;
        }


        var w = GameController.Main.WorldController;

        position.y = w.World.GetHeight(position);

        var s = Instantiate<Structure>(structure, PlayerStructures.transform);
        s.transform.position = position;
        s.builder = GameController.Main.UIController.StratView.inspectee.GetComponent<Builder_Unit>();

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
                if (h2 != h)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
