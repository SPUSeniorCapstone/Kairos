using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Main
    {
        get
        {
            if(main = null)
            {
                main = FindObjectOfType<GameController>();
            }
            return main;
        }
    }
    static GameController main;

    public InputController inputController;

    public SelectionController selectionController;

    public bool paused;
    public CursorLockMode defaultLockMode = CursorLockMode.None;

    // awake or start?
    private void Awake()
    {
        Main = this;
        inputController = new InputController();
    }
}
