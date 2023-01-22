using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class Boid : Unit
{

    public Vector3 velocity = Vector3.zero;
    //public List<GameObject> boids;
    // Start is called before the first frame update
    void Start()
    {
        boidStart();
       //var arry = FindObjectsOfType<Boid>();
       // foreach (var go in arry)
       // {
       //     boids.Add(go.gameObject);
       // }
       // boids.Remove(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MoveAsync(Vector3 mapPosition)
    {
       
    }

    void RotateTowardsMovement(Vector3 move)
    {
      
    }
    public void Move(Vector3 mapPosition)
    {

       

    }


}
