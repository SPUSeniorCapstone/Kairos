using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionMarker : MonoBehaviour
{
    public Vector3 point = new Vector3();
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void PlaceMarker()
    {
        transform.position = point;
        gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
