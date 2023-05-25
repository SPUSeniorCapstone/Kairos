using System;
using UnityEngine;

[Serializable]
public class InputController
{

    public InputType MoveForward = new InputType(KeyCode.W, KeyCode.UpArrow);
    public InputType MoveBack = new InputType(KeyCode.S, KeyCode.DownArrow);
    public InputType MoveRight = new InputType(KeyCode.D, KeyCode.RightArrow);
    public InputType MoveLeft = new InputType(KeyCode.A, KeyCode.LeftArrow);
    // proper syntax?
    public InputType Select = new InputType(KeyCode.Mouse0, KeyCode.None);
    public InputType Command = new InputType(KeyCode.Mouse1, KeyCode.None);
    // camera
    public InputType ZoomIn = new InputType(KeyCode.E, KeyCode.None);
    public InputType ZoomOut = new InputType(KeyCode.Q, KeyCode.None);
    public InputType RotateCamera = new InputType(KeyCode.Mouse2, KeyCode.None);

    //HealthBars
    public InputType ToggleHealthBars = new InputType(KeyCode.H, KeyCode.None);


    public InputType Pause = new InputType(KeyCode.Escape, KeyCode.None);
    public InputType HideUI = new InputType(KeyCode.Semicolon, KeyCode.None);

    // hotkeys
    public InputType Zero = new InputType(KeyCode.Alpha0, KeyCode.None);
    public InputType One = new InputType(KeyCode.Alpha1, KeyCode.None);
    public InputType Two = new InputType(KeyCode.Alpha2, KeyCode.None);
    public InputType Three = new InputType(KeyCode.Alpha3, KeyCode.None);
    public InputType Four = new InputType(KeyCode.Alpha4, KeyCode.None);
    public InputType Five = new InputType(KeyCode.Alpha5, KeyCode.None);
    public InputType Six = new InputType(KeyCode.Alpha6, KeyCode.None);
    public InputType Seven = new InputType(KeyCode.Alpha7, KeyCode.None);
    public InputType Eight = new InputType(KeyCode.Alpha8, KeyCode.None);
    public InputType Nine = new InputType(KeyCode.Alpha9, KeyCode.None);

    public InputType Stop = new InputType(KeyCode.S, KeyCode.None);
    public InputType AttackMove = new InputType(KeyCode.A, KeyCode.None);
    public InputType AllFightingUnits = new InputType(KeyCode.Q, KeyCode.None);
    public InputType SameUnitsOnMap = new InputType(KeyCode.E, KeyCode.None);
    // avoid corruption squares?
    public InputType Scatter = new InputType(KeyCode.X, KeyCode.None);
    public InputType SeeStronghold = new InputType(KeyCode.H, KeyCode.None);


    [Serializable]
    public struct InputType
    {
        public enum HoldKey
        {

        }

        [SerializeField]
        KeyCode key1;

        [SerializeField]
        KeyCode key2;



        public InputType(KeyCode primary, KeyCode secondary)
        {
            key1 = primary;
            key2 = secondary;
        }

        /// <summary>
        /// returns true during the frame the key is pressed
        /// </summary>
        public bool Down()
        {
            if (Input.GetKeyDown(key1) || Input.GetKeyDown(key2))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// returns true while the key is held down
        /// </summary>
        public bool Pressed()
        {
            if (Input.GetKey(key1) || Input.GetKey(key2))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// returns true during the frame the user releases the key
        /// </summary>
        public bool KeyUp()
        {
            if (Input.GetKeyUp(key1) || Input.GetKeyUp(key2))
            {
                return true;
            }
            return false;
        }
    }
}

