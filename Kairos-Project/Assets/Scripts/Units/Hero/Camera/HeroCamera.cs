using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCamera : MonoBehaviour
{
    public GameObject player;
    public float autoRotateSpeed = 1;
    public float modelHeight = 1.5f;

    public float rotateSpeed = 1;
    public float zoomSpeed = 1;
    public float moveSpeed = 1;

    public bool rotateCamera = true;

    public float cameraHeight = 8;
    public float cameraDistance = 10;

    private void Start()
    {
    }

    void Update()
    {
        ZoomWithScroll();
        if (Input.GetMouseButton(1))
        {
            RotateWithMouse();
        }
        else
        {
            RotateTowardsPlayer();
        }
        MoveTowardsPlayer();
        
        
    }

    void ZoomWithScroll()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            cameraHeight -= Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;

            if(Input.mouseScrollDelta.y > 0)
            {
                if (cameraHeight < 5)
                {
                    cameraDistance -= Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;
                    if (cameraDistance < 4)
                    {
                        cameraDistance = 4;
                    }
                }
            }
            else
            {
                cameraDistance -= Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime;
                if(cameraDistance > 10)
                {
                    cameraDistance = 10;
                }
            }

            

            if (cameraHeight < 2)
            {
                cameraHeight = 2;
            }
            if (cameraHeight > 30)
            {
                cameraHeight = 30;
            }

            //RotateTowardsPlayer();
        }
    }

    void RotateWithMouse()
    {
        float rotateAmount = (Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime);
        transform.RotateAround(player.transform.position, Vector3.up, rotateAmount);
        float rotateY = (Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime);
        transform.RotateAround(transform.position, transform.right, rotateY);
    }

    void RotateTowardsPlayer()
    {
        Quaternion rotation = transform.rotation;

        Vector3 target = player.transform.position + (Vector3.up * modelHeight);

        Vector3 direction = target - transform.position;
        var lookRotation = Quaternion.LookRotation(direction);

        rotation = Quaternion.Slerp(rotation, lookRotation, Time.deltaTime * autoRotateSpeed);
        transform.rotation = rotation;
    }

    void MoveTowardsPlayer()
    {
        Vector3 newPosition = transform.position;
        Vector3 target = player.transform.position;
        



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
}
