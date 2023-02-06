using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandGroup : MonoBehaviour
{
    public List<Entity> entities;

    public int speedLimit;
    public Vector3 max, min;
    int Xmin, Xmax, Ymin, Ymax, Zmin, Zmax;
    public Vector3 centerVector;
    public float followStr;
    public float followSpeed;
    //public GameObject centerObj;
    public GameObject groupTargetObj;
    public bool selected;
    public bool lesser = true;

    public bool flock;

    public bool twoD = false;
    // old max 0.25f
    [Range(0, 1f)]
    public float alignmentFactor;

    [Range(0, 50f)]
    public float effectiveDistance;

    [Range(0, 1f)]
    public float cohesionFactor;

    [Range(0, 1f)]
    public float boundFactor;

    [Range(0, 100f)]
    public float distanceFromTarget;


    // Start is called before the first frame update
    void Start()
    {
        //var arry = GetComponentsInChildren<Entity>();
        //if (arry.Length != 0)
        //{
        //    foreach (var go in arry)
        //    {
        //        entities.Add(go);
        //        go.targetObject = groupTargetObj;
        //    }
        //}
        //var joe = GetComponent<Entity>();
        //if (joe != null && arry.Length == 0)
        //{
        //    Debug.Log("REEEEE");
        //    entities.Add(joe);
        //    joe.targetObject = groupTargetObj;
        //}
    

    }

    // Update is called once per frame
    void Update()
    {
        CalculateCenter();
        // is there a better way?
        if (lesser)
        {
            CheckIfEmpty();
        }
        else if (!lesser)
        {
            foreach (CommandGroup group in GameController.Main.CommandController.commandGroups)
            {
                if (group != this)
                {
                    group.effectiveDistance = effectiveDistance;
                    group.distanceFromTarget = distanceFromTarget;
                    group.boundFactor = boundFactor;
                    group.cohesionFactor = cohesionFactor;
                    group.alignmentFactor = alignmentFactor;
                    group.followSpeed = followSpeed;
                    group.followStr = followStr;
                }
            }
        }
    }

    public void CalculateCenter()
    {
        float x = 0;
        float y = 0;
        float z = 0;
        foreach (Entity entity in entities)
        {
            entity.Perching();
            x += entity.transform.position.x;
            y += entity.transform.position.y;
            z += entity.transform.position.z;
            //avgAlignment += Alignment(tree);

        }
        //avgAlignment = avgAlignment.normalized;
        x /= entities.Count;
        //y /= boids.Count;
        y = 0f;
        z /= entities.Count;
        centerVector = new Vector3(x, y, z);
        if (entities.Count == 0){
            centerVector = Vector3.zero;
        }
        transform.position = centerVector;
        //centerObj.transform.LookAt(pos, transform.up);

    }
    public void SetGroupTarget(GameObject gameObject)
    {
        groupTargetObj = gameObject;
        foreach (Entity entity in entities)
        {
            entity.targetObject = groupTargetObj;
            entity.SetTargetPos();
            entity.idle = false;
        }
    }
    public void CheckIfEmpty()
    {
        if (entities.Count == 0)
        {
            GameController.Main.CommandController.commandGroups.Remove(this);
            Destroy(gameObject);
        }
    }
}
