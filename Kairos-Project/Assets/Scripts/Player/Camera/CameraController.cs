using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 1;
    private float oldSpeed = 1;
    public float rotateSpeed = 1;
    public float verticalSpeed = 1;

    public Vector3 defaultRotation = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        oldSpeed = moveSpeed;
        if(defaultRotation == Vector3.zero)
        {
            defaultRotation = transform.rotation.eulerAngles;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.main.paused)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {

                transform.rotation = Quaternion.Euler(defaultRotation);
            }

            MoveCamera();
            RotateCamera();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            moveSpeed = moveSpeed * 2;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            moveSpeed = oldSpeed;
        }
    }

    void MoveCamera()
    {
        float inputX = Input.GetAxis("Vertical");
        float inputZ = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Jump");

        Vector3 up = inputY * Vector3.up;
        Vector3 forward = inputX * transform.forward;
        Vector3 right = inputZ * transform.right;
        forward.y = 0;
        forward = forward.normalized;
        right.y = 0;
        right = right.normalized;



        transform.position += (forward + right) * Time.deltaTime * moveSpeed;
        transform.position += up * Time.deltaTime * verticalSpeed;
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
