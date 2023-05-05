using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileArrow : MonoBehaviour
{
    public GameObject target;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        //transform.position.Set(0,6,0);
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null && speed != 0f) {
            Vector3 targetHeight = target.transform.position;
            if (target.tag != "Spider")
            {
                targetHeight.y += 1;
            }
      
            transform.position = Vector3.MoveTowards(transform.position, targetHeight, speed);
            transform.LookAt(targetHeight);
            Vector3 flip = new Vector3(0,180,0);
            transform.Rotate(flip);
            if (Vector3.Distance(transform.position, targetHeight) < 1)
            {
                Destroy(gameObject);
            }
        } else if (target == null && speed != 0f)
        {
            Destroy(gameObject);
        }
    }
}
