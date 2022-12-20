using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraMode {
        /// <summary>
        /// Free Roam Isometric Camera
        /// </summary>
        FREE,
        /// <summary>
        /// Hero First Person Camera **Not implimented**
        /// </summary>
        HERO_FIRST_PERSON,
        /// <summary>
        /// Hero Third Person Camera
        /// </summary>
        HERO_THIRD_PERSON
    }

    public HeroCamera heroCam;
    public FreeCamera freeCam;

    [Disable]
    public CameraMode cameraMode = CameraMode.FREE;

    private void Start()
    {
        SwapCamera(cameraMode);
    }

    private void Update()
    {
        
    }

    public void SwapCamera(CameraMode cameraMode)
    {
        switch (cameraMode)
        {
            case CameraMode.FREE:
                heroCam.gameObject.SetActive(false);
                freeCam.gameObject.SetActive(true);
                break;
            case CameraMode.HERO_FIRST_PERSON:
                //heroCam.gameObject.SetActive(false);
                //freeCam.gameObject.SetActive(false);
                Debug.Log("Not Yet Implimented");
                return;
                //break;
            case CameraMode.HERO_THIRD_PERSON:
                heroCam.gameObject.SetActive(true);
                freeCam.gameObject.SetActive(false);
                cameraMode = CameraMode.HERO_THIRD_PERSON;
                break;
            default:
                Debug.LogError("Invalid State");
                return;
        }

        this.cameraMode = cameraMode;
    }

}
