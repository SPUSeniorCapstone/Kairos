using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureController : MonoBehaviour
{
    public bool StructurePlacementMode = false;

    public List<Structure> masterStructure;

    public List<Structure> previewList;

    // don't know how else to do this
    public GameObject PlayerStructures;
    public GameObject EnemyStructures;

    public GameObject PlayerUnits;
    public GameObject EnemyUnits;

    

    public Structure StructureToSpawn;
    public GameObject structurePreview;

    public Structure corruptionNode;

    public ProductionStructure selected;

    public List<Structure> CorruptionNodes = new List<Structure>();
    public Structure StrongholdActual;
    public Structure StrongholdPrefab;

    // PLEASE FIX THIS
    public GameObject infantry;
    public GameObject archer;
    public GameObject builder;
    public GameObject resourceCollector;

    // previews
    public GameObject sp;
    public GameObject bp;
    public GameObject at;
    public GameObject rs;

    public Material valid;
    public Material invalid;

    private void Start()
    {
        StrongholdActual = PlaceStructure(StrongholdPrefab, GameController.Main.WorldController.WorldGenerator.strongholdPos);
        Vector3 spawn = StrongholdActual.transform.position;
        spawn.x += 6;
        Instantiate(builder, spawn, Quaternion.identity, PlayerUnits.transform);
        foreach (var pos in GameController.Main.WorldController.WorldGenerator.corruptionNodePositions)
        {
            CorruptionNodes.Add(PlaceStructure(corruptionNode, pos));
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



            // i want this code to have active feedback for good placement
            structurePreview.transform.position = pos;
            if (!IsValidPlacement(structurePreview.GetComponent<Structure>(), pos))
            {
                structurePreview.GetComponentInChildren<MeshRenderer>().material = invalid;
            }
            else
            {
                structurePreview.GetComponentInChildren<MeshRenderer>().material = valid;
            }
           

            if (GameController.Main.inputController.Select.Pressed() && GameController.Main.UIController.StratView.inspectee.GetComponent<Builder_Unit>() != null)
            {
                StructurePlacementMode = false;
                GameController.Main.SelectionController.testCooldown = false;
                GameController.Main.UIController.StratView.inspectee.GetComponent<Builder_Unit>().BuildTask(pos);
                var s = PlaceStructure(StructureToSpawn, pos);
                s.builder = GameController.Main.UIController.StratView.inspectee.GetComponent<Builder_Unit>();
                structurePreview.transform.position = Vector3.zero;
                GameController.Main.SelectionController.testCooldown = true;
            }
        }
    }

    public void BuildOrder(string name)
    {
        StructurePlacementMode = true;
        sp.SetActive(false);
        bp.SetActive(false);
        at.SetActive(false);
        if(name == sp.name)
        {
            sp.SetActive(true);
            StructureToSpawn = previewList.ElementAt(0);

        }
        else if(name == bp.name)
        {
            bp.SetActive(true);
            StructureToSpawn = previewList.ElementAt(1);
        }
        else if (name == at.name)
        {
            Debug.Log("AKDKA");
            at.SetActive(true);
            StructureToSpawn = previewList.ElementAt(2);
        }
        
    }

    public void TrainArcher()
    {
        selected.QueueUnits(archer);
    }
    public void TrainInfantry()
    {
        selected.QueueUnits(infantry);
    }
    public void TrainBuilder()
    {
        selected.QueueUnits(builder);
    }
    public void TrainCollector()
    {
        selected.QueueUnits(resourceCollector);
    }

    public Structure PlaceStructure(Structure structure, Vector3Int position)
    {
        if (!IsValidPlacement(structure, position))
        {
            Debug.Log("Invalid Structure Placement");
            return null;
        }


        var w = GameController.Main.WorldController;

        position.y = w.World.GetHeight(position);

        Structure s;
        if (structure.GetComponent<Selectable>().faction)
        {
             s = Instantiate<Structure>(structure, EnemyStructures.transform);
        }
        else
        {
             s = Instantiate<Structure>(structure, PlayerStructures.transform);
        }
     
        s.transform.position = position;
        // need to fix, enemy buildings call this with no builder
 

        for (int x = 0; x < structure.Size.x; x++)
        {
            for (int z = 0; z < structure.Size.z; z++)
            {
                int h = w.World.GetHeight(position.x + x, position.z + z);
                w.World.SetHeight(position.x + x, position.z + z, h + structure.Size.y);
            }
        }

        return s;

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
                if (structure.GetComponent<ResourceStructure>() != null)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
