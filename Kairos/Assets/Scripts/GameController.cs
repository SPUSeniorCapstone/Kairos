using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Main
    {
        get
        {
            if (main == null)
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
            if (inputController == null)
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
            if (selectionController == null)
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
            if (worldController == null)
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
            if (damageableController == null)
            {
                damageableController = FindObjectOfType<DamageableController>();
            }
            return damageableController;
        }
    }
    DamageableController damageableController;

    public MenuController menuController;


    public CorruptionController CorruptionController
    {
        get
        {
            if (corruptionController == null)
            {
                corruptionController = FindObjectOfType<CorruptionController>();
            }
            return corruptionController;
        }
    }
    CorruptionController corruptionController;

    // misnomar, simply cleans up any lists it might be in before letting the original caller destroy itself
    public void MasterDestory(GameObject item)
    {
        if (EntityController == null || SelectionController == null)
        {
            return;
        }

        Entity entity = item.GetComponent<Entity>();
        Selectable selectable = item.GetComponent<Selectable>();
        Unit unit = item.GetComponent<Unit>();

        if (selectable == null)
        {
            selectable = item.GetComponentInChildren<Selectable>();
        }
        if (unit != null)
        {
            EntityController.Entities.Remove(entity);
            Destroy(unit);
            Destroy(entity);
        }
        // un comment to put back as master destroy
        SelectionController.masterSelect.Remove(selectable);
        SelectionController.currentlySelect.Remove(selectable);

    }

    public bool lost;
    public bool won;
    public int playerCount = 0;
    public int enemyCount = 0;
    public bool WinConDebug = false;

    public int resouceCount = 0;

    public void CheckVictory(Structure structure)
    {
        if (!WinConDebug)
        {
            if (structure != null && structure.enemy)
            {
                enemyCount--;
                UIController.gameUI.UpdateNodes(enemyCount);
            }
            else if (structure != null)
            { 
                playerCount--;
            }
            // does second clause ensure survival while builder lives?
            //&& FindAnyObjectByType<Builder_Unit>().GetComponent<Damageable>() != null && FindAnyObjectByType<Builder_Unit>().GetComponent<Damageable>().Dead
            if (playerCount <= 0 && FindAnyObjectByType<Builder_Unit>() == null)
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
    }

    public void Pause(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0f;
            paused = true;
        }
        else
        {
            Time.timeScale = 1f;
            paused = false;
        }
    }

    public void Start()
    {
        GameController.Main.menuController.Resume();
    }

    private void Update()
    {
        if (InputController.Pause.Down())
        {
            Pause(!paused);
        }
    }

    public bool paused;
    public CursorLockMode defaultLockMode = CursorLockMode.None;
    public Material DeathMaterial;
    public Shader highlight;
    public Shader unHighlight;
    public CommandGroup CGSettings;
    public bool randomDamageModifier = false;

    public List<Builder_Unit> masterBuilder = new List<Builder_Unit>();

        public void UpdateResource(int count)
    {
        resouceCount += count;
        Main.UIController.gameUI.ResourceCounter.text = Main.UIController.gameUI.FormatNum(resouceCount, true);
/*        ResourceCounter.text = FormatNum(count, true);
*/    }
}
