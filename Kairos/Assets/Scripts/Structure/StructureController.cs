using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StructureController : MonoBehaviour
{
    public bool StructurePlacementMode = false;
    public bool cancel;

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

    public List<ProductionStructure> selected = new List<ProductionStructure>();

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
    public GameObject p;

    public Material valid;
    public Material invalid;

    private void Start()
    {
        StrongholdActual = PlaceStructure(StrongholdPrefab, GameController.Main.WorldController.WorldGenerator.strongholdPos);
        Vector3 spawn = StrongholdActual.transform.position;
        spawn.x += 6;
        Instantiate(builder, spawn, Quaternion.identity, PlayerUnits.transform);
        spawn.x += 1;
        Instantiate(resourceCollector, spawn, Quaternion.identity, PlayerUnits.transform);
        foreach (var pos in GameController.Main.WorldController.WorldGenerator.corruptionNodePositions)
        {
            CorruptionNodes.Add(PlaceStructure(corruptionNode, pos));
        }
        GameController.Main.UIController.gameUI.UpdateNodes(CorruptionNodes.Count);
    }

    void Update()
    {
        if (selected.Count == 0)
        {
            GameController.Main.UIController.EnableProductionMenu(false);
        }
        else
        {
            GameController.Main.UIController.EnableProductionMenu(true);
        }
        if (StructurePlacementMode)
        {
            structurePreview.SetActive(true);
            // weird, if multiple asking for input controller, doesnt work
            // GameController.Main.InputController.Command.Down()
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Debug.Log("why no work?");
                
                structurePreview.transform.position = Vector3.zero;
                structurePreview.SetActive(false);
                StructurePlacementMode = false;
                cancel = true;

            }
            else
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


                if (GameController.Main.inputController.Select.Pressed() && GameController.Main.UIController.StratView.inspectee.GetComponent<Builder_Unit>() != null && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
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
        else
        {
            cancel = false;
        }
    }

    public void SellStructure()
    {
        if (GameController.Main.UIController.StratView.inspectee != null && GameController.Main.UIController.StratView.inspectee.GetComponent<Structure>() != null && GameController.Main.UIController.StratView.inspectee.GetComponent<Damageable>() != null)
        {
            Damageable damageable = GameController.Main.UIController.StratView.inspectee.GetComponent<Damageable>();
            damageable.Damage(damageable.MaxHealth);
        }
    }

    public void BuildOrder(string name)
    {
        StructurePlacementMode = true;
        sp.SetActive(false);
        bp.SetActive(false);
        at.SetActive(false);
        p.SetActive(false);
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
            at.SetActive(true);
            StructureToSpawn = previewList.ElementAt(2);
        } 
        else if (name == p.name)
        {
            p.SetActive(true);
            StructureToSpawn = previewList.ElementAt(3);
        }
        
    }

    public void TrainArcher()
    {
        foreach(ProductionStructure s in selected)
        {
            s.archerCount++;
            s.QueueUnits(archer);
        }
    }
    public void TrainInfantry()
    {
        foreach (ProductionStructure s in selected)
        {
            s.infantryCount++;
            s.QueueUnits(infantry);
        }
    }
    public void TrainBuilder()
    {
        foreach (ProductionStructure s in selected)
        {
            s.builderCount++;
            s.QueueUnits(builder);
        }
    }
    public void TrainCollector()
    {
        foreach (ProductionStructure s in selected)
        {
            s.rcCount++;
            s.QueueUnits(resourceCollector);
        }
    }
    public void UntrainInfantry(int refund)
    {
        foreach (ProductionStructure s in selected)
        {
            if (s.infantryCount > 0)
            {
                s.infantryCount--;
                s.DequeueUnits(infantry);
                GameController.Main.UpdateResource(-refund);
            }
        }
    }
    public void UntrainArcher(int refund)
    {
        foreach (ProductionStructure s in selected)
        {
            if (s.archerCount > 0)
            {
                s.archerCount--;
                s.DequeueUnits(archer);
                GameController.Main.UpdateResource(-refund);
            }
        }
    }
    public void UntrainBuilder(int refund)
    {
        foreach (ProductionStructure s in selected)
        {
            if (s.builderCount > 0)
            {
                s.builderCount--;
                s.DequeueUnits(builder);
                GameController.Main.UpdateResource(-refund);
            }
        }
    }
    public void UntrainCollector(int refund)
    {
        foreach (ProductionStructure s in selected)
        {
            if (s.rcCount > 0)
            {
                s.rcCount--;
                s.DequeueUnits(resourceCollector);
                GameController.Main.UpdateResource(-refund);
            }
        }
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
