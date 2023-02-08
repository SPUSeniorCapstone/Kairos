using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Entity> masterEntitiy;
    void Start()
    {
        masterEntitiy = FindObjectsOfType<Entity>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
