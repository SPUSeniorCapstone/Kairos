using UnityEngine;

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

        transform.position = pos +Vector3.up * 10;
    }
}
