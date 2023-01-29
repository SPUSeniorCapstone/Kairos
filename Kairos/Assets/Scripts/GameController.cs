using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController main;

    public InputController inputController;

    public SelectionController selectionController;

    public bool paused;
    public CursorLockMode defaultLockMode = CursorLockMode.None;

    // awake or start?
    private void Awake()
    {
        main = this;
        inputController = new InputController();
    }
}
