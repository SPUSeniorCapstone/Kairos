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

    public bool paused;
    public CursorLockMode defaultLockMode = CursorLockMode.None;
}
