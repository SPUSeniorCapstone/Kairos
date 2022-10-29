using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 1;
    public float rotateSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.main.paused)
        {
            MoveCamera();
            RotateCamera();
        }
    }

    void MoveCamera()
    {
        float inputX = Input.GetAxis("Horizontal") * moveSpeed;
        float inputZ = Input.GetAxis("Vertical") * moveSpeed;
        float inputY = 0.0f;

        if (Input.GetKey(KeyCode.Q))
        {
            inputY = -moveSpeed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            inputY = moveSpeed;
        }

        transform.position += new Vector3(inputX, inputY, inputZ) * Time.deltaTime;
    }

    void RotateCamera()
    {
        if (Input.GetMouseButton(2))
        {
            Cursor.lockState = CursorLockMode.Locked;
            transform.eulerAngles += rotateSpeed * new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime;
        }
        else
        {
            Cursor.lockState = GameController.main.defaultLockMode;
        }
    }
}
