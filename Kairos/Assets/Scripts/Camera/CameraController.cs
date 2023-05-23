using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera RTSCamera;
    public Camera OverviewCamera;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            RTSCamera.gameObject.SetActive(!RTSCamera.gameObject.activeSelf);
            OverviewCamera.gameObject.SetActive(!OverviewCamera.gameObject.activeSelf);        
        }
    }
}
