using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EntityController : MonoBehaviour
{

    public List<Entity> Entities
    {
        get { return masterEntityList; }
    }
    [SerializeField] List<Entity> masterEntityList = new List<Entity>();
    HashSet<Entity> entityHash = new HashSet<Entity>();

    public void AddEntity(Entity entity)
    {
        if (entityHash.Contains(entity))
        {
            Debug.LogError("Cannot insert same entity twice");
            return;
        }
        entityHash.Add(entity);
        masterEntityList.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        if (entityHash.Contains(entity))
        {
            Debug.Log("Entity Controller does not contain: " + entity);
            return;
        }
        entityHash.Remove(entity);
        masterEntityList.Remove(entity);
    }
}
