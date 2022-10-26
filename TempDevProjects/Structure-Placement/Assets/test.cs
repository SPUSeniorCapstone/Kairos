using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField]
    private float speed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.C))
        {
            //transform.position = new Vector3(100, 100, 100);
            // go up
            transform.position = transform.position + Vector3.up * speed * Time.deltaTime; // 0, 1, 0

        }
        
        if(Input.GetKey(KeyCode.V))
        {
            // go down
            transform.position = transform.position + Vector3.down * speed * Time.deltaTime;
        }

        // forward
        if(Input.GetKey(KeyCode.W))
        {
            // go forward
            transform.position = transform.position + Vector3.forward * speed * Time.deltaTime; 
        }

        // backward
        if (Input.GetKey(KeyCode.S))
        {
            // go forward
            transform.position = transform.position + Vector3.back * speed * Time.deltaTime;
        }

        // left
        if(Input.GetKey(KeyCode.A))
        {
            // go left
            transform.position = transform.position - Vector3.left * speed * Time.deltaTime;
        }

        // right
        if (Input.GetKey(KeyCode.D))
        {
            // go left
            transform.position = transform.position - Vector3.right * speed * Time.deltaTime;
        }

    }
}
