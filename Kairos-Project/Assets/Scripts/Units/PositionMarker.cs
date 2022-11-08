using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionMarker : MonoBehaviour
{
    public Vector3 point = new Vector3();
    private int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void PlaceMarker()
    {
        transform.position = point;
        gameObject.SetActive(true);
        count = 0;
        //Debug.Log(transform.position + "Coords");
    }
    // Update is called once per frame
    void Update()
    {
        count++;
        if(count == 300)
        {
            gameObject.SetActive(false);
        }
    }
}
