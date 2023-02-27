using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Main
    {
        get
        {
            if(main == null)
            {
                main = FindObjectOfType<GameController>();
            }
            return main;
        }
    }
    static GameController main;

    public InputController InputController
    {
        get
        {
            if(inputController == null)
            {
                inputController = new InputController();
            }
            return inputController;
        }
    }
    [SerializeField] public InputController inputController;

    public PathFinder PathFinder
    {
        get
        {
            if (pathFinder == null)
            {
                pathFinder = new PathFinder();
            }
            return pathFinder;
        }
    }
    [SerializeField] PathFinder pathFinder;

    public SelectionController SelectionController
    {
        get
        {
            if(selectionController == null)
            {
                selectionController = FindObjectOfType<SelectionController>();
            }
            return selectionController;
        }
    }
    SelectionController selectionController;

    public WorldController WorldController
    {
        get
        {
            if(worldController == null)
            {
                worldController = FindObjectOfType<WorldController>();
            }
            return worldController;
        }
    }
    WorldController worldController;

    public CommandController CommandController 

    {
        get
        {
            if (commandController == null)
            {
                commandController = FindObjectOfType<CommandController>();
            }
            return commandController;
        }
    }
    CommandController commandController;

    public EntityController EntityController

    {
        get
        {
            if (entityController == null)
            {
                entityController = FindObjectOfType<EntityController>();
            }
            return entityController;
        }
    }
    EntityController entityController;

    public UIController UIController
    {
        get
        {
            if (uiController == null)
            {
                uiController = FindObjectOfType<UIController>();
            }
            return uiController;
        }
    }
    [SerializeField] UIController uiController;

    public StructureController StructureController
    {
        get
        {
            if (structureController == null)
            {
                structureController = FindObjectOfType<StructureController>();
            }
            return structureController;
        }
    }
    [SerializeField] StructureController structureController;

    public DamageableController DamageableController
    {
        get
        {
            if(damageableController == null)
            {
                damageableController = FindObjectOfType<DamageableController>();
            }
            return damageableController;
        }
    }
    DamageableController damageableController;

    public MenuController menuController;

    public void MasterDestory(GameObject item)
    {
        if(EntityController == null || SelectionController == null)
        {
            return;
        }

       Entity entity = item.GetComponent<Entity>();
        Selectable selectable = item.GetComponent<Selectable>();
        
        if (selectable == null)
        {
            selectable = item.GetComponentInChildren<Selectable>();
        }
        EntityController.Entities.Remove(entity);
        SelectionController.masterSelect.Remove(selectable);
        SelectionController.currentlySelect.Remove(selectable);
        
    }

    public bool lost;
    public bool won;
    public int playerCount = 0;
    public int enemyCount = 0;

    public void CheckVictory(Structure structure)
    {
        if (structure.GetComponent<Selectable>().faction)
        {
            enemyCount--;
        }
        else
        {
            playerCount--;
        }
        if (playerCount <= 0)
        {
            lost = true;
            menuController.Defeat();
            
        }
        else if (enemyCount <= 0)
        {
            won = true;
            menuController.Victory();
        }
    }
    public void Start()
    {
        GameController.Main.menuController.Resume();
    }

    public bool paused;
    public CursorLockMode defaultLockMode = CursorLockMode.None;
}
