using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : Entity
{
    //May need updating if we want more complex shapes
    public int width, length;

    //south-west (lower-left) corner of the structure 
    Vector2Int gridPosition;

    public override float MaxHealth { get; protected set; }
    public override float Health { get; protected set; }


    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
