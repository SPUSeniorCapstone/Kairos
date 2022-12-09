using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public static CursorController main;
    public Texture2D capture;

    private void Start()
    {
       
        capture = (Texture2D)Resources.Load("Textures/Cursors/Capture_Cursor");
        main = this;
    }

    public enum CursorTexture
    {
        UNIT, STRUCTURE, ENEMY, HERO, NPC, RESOURCE, BUILDPLOT 
     
    }
    public void ChangeCursor(CursorTexture texture){ 
        //switch(texture){
        //    case CursorTexture.UNIT: {
        //            Cursor.SetCursor(null);  break;}
    }
}
