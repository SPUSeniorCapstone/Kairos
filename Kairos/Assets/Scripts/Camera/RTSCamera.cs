using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    public Vector3 position;
    public float autoRotateSpeed = 1;
    public float modelHeight = 1.5f;

    public float rotateSpeed = 1;
    public float zoomSpeed = 1;
    public float moveSpeed = 1;
    public float verticalSpeed = 1;

    public bool rotateCamera = true;


    public float maxHeight = 50;
    public float minHeight = 8;

    
    public float cameraHeight = 20;
    [DisableOnPlay]
    public float cameraDistance = 30;


    [Range(0.05f, 0.95f)]
    public float curveFactor = 0.5f;

    public Vector3 defaultRotation = Vector3.zero;


    private float oldSpeed = 1;

    void Start()
    {
        oldSpeed = moveSpeed;
        if (defaultRotation == Vector3.zero)
        {
            defaultRotation = transform.rotation.eulerAngles;
        }
        FocusOnPosition(GameController.Main.StructureController.StrongholdActual.transform.position);
    }

    void Update()
    {
        if (!GameController.Main.paused)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {

                transform.rotation = Quaternion.Euler(defaultRotation);
            }

            MoveCamera();

            position.y = WorldController.Main.GetHeight(position);

            ZoomWithScroll();
            if (GameController.Main.inputController.RotateCamera.Pressed())
            {
                RotateWithMouse();
            }

            MoveTowardsPosition();
            RotateTowardsPlayer();
        }
    }

    void ZoomWithScroll()
    {
        cameraHeight -= Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;

        if (cameraHeight < minHeight)
        {
            cameraHeight = minHeight;
        }
        if (cameraHeight > maxHeight)
        {
            cameraHeight = maxHeight;
        }

        cameraDistance = cameraHeight / curveFactor;
    }

    void RotateWithMouse()
    {
        float rotateAmount = (Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime);
        transform.RotateAround(position, Vector3.up, rotateAmount);
    }

    void RotateTowardsPlayer()
    {
        Quaternion rotation = transform.rotation;

        Vector3 target = position + (Vector3.up * modelHeight);

        Vector3 direction = target - transform.position;
        var lookRotation = Quaternion.LookRotation(direction);

        rotation = Quaternion.Slerp(rotation, lookRotation, Time.deltaTime * autoRotateSpeed);
        transform.rotation = rotation;
    }

    void MoveTowardsPosition()
    {
        Vector3 newPosition = transform.position;
        Vector3 target = position;




        Vector3 dir;
        if (rotateCamera)
        {
            Vector3 temp = newPosition;
            temp.y = target.y;

            dir = (target - temp).normalized;
        }
        else
        {
            dir = transform.forward;
            dir.y = 0;
        }

        newPosition = target - (dir * cameraDistance);
        newPosition.y = target.y + cameraHeight;

        newPosition = Vector3.Slerp(transform.position, newPosition, moveSpeed * Time.deltaTime);
        transform.position = newPosition;
    }

    void MoveCamera()
    {
        float horz = 0;
        if (GameController.Main.InputController.ZoomIn.Pressed())
        {
            horz = 1;
        }
        if (GameController.Main.InputController.ZoomOut.Pressed())
        {
            horz = -1;
        }

        Vector3 forward = new Vector3();
        if (GameController.Main.InputController.MoveForward.Pressed())
        {
            forward = transform.forward;
        }
        if (GameController.Main.InputController.MoveBack.Pressed())
        {
            forward = transform.forward * -1;
        }

        Vector3 right = new Vector3();
        if (GameController.Main.InputController.MoveRight.Pressed())
        {
            right = transform.right;
        }
        if (GameController.Main.InputController.MoveLeft.Pressed())
        {
            right = transform.right * -1;
        }

        forward.y = 0;
        forward = forward.normalized;
        right.y = 0;
        right = right.normalized;

        position += (forward + right) * Time.deltaTime * moveSpeed;
        cameraHeight += horz * Time.deltaTime * verticalSpeed;
    }

    public Vector3 GetXZForward()
    {
        Vector3 dir = transform.forward;
        dir.y = 0;
        dir = dir.normalized;
        return dir;
    }

    public Vector3 GetXZRight()
    {
        Vector3 dir = transform.right;
        dir.y = 0;
        dir = dir.normalized;
        return dir;
    }

    void FocusOnPosition(Vector3 pos)
    {
        position = new Vector3(pos.x,WorldController.Main.GetHeight(pos),pos.z);
    }
}

/*
public class RTSCamera : MonoBehaviour
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
        if (defaultRotation == Vector3.zero)
        {
            defaultRotation = transform.rotation.eulerAngles;
        }
        FocusOnPosition(GameController.Main.StructureController.StrongholdActual.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameController.Main.paused)
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
        Vector3 horz = new Vector3();
        if (GameController.Main.InputController.ZoomIn.Pressed())
        {
            horz = Vector3.down;
        }
        if (GameController.Main.InputController.ZoomOut.Pressed())
        {
            horz = Vector3.up;
        }

        Vector3 forward = new Vector3();
        if (GameController.Main.InputController.MoveForward.Pressed())
        {
            forward = transform.forward;
        }
        if (GameController.Main.InputController.MoveBack.Pressed())
        {
            forward = transform.forward * -1;
        }

        Vector3 right = new Vector3();
        if (GameController.Main.InputController.MoveRight.Pressed())
        {
            right = transform.right;
        }
        if (GameController.Main.InputController.MoveLeft.Pressed())
        {
            right = transform.right * -1;
        }

        forward.y = 0;
        forward = forward.normalized;
        right.y = 0;
        right = right.normalized;

        transform.position += (forward + right) * Time.deltaTime * moveSpeed;
        transform.position += horz * Time.deltaTime * verticalSpeed;
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
            Cursor.lockState = GameController.Main.defaultLockMode;
        }
    }

    void FocusOnPosition(Vector3 pos)
    {
        //pos = new Vector3(pos.x, WorldController.Main.World.GetHeight(pos.x, pos.z), pos.z);

        transform.position = pos +Vector3.up * 30;
    }
}
*/