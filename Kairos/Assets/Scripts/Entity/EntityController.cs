using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{

    /// <summary>
    /// Entity Master List
    /// <para>All entities in use should appear in this list</para>
    /// </summary>
    public List<Entity> Entities
    {
        get { return masterEntityList; }
    }
    [SerializeField] List<Entity> masterEntityList = new List<Entity>();
    HashSet<Entity> entityHash = new HashSet<Entity>();

    /// <summary>
    /// Adds an entity to the master list of Entities
    /// <para>Should be called in the start method of any entity</para>
    /// </summary>
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

    /// <summary>
    /// Adds an entity to the master list of Entities
    /// <para>Should be called whenever an entity is destroyed</para>
    /// </summary>
    public void RemoveEntity(Entity entity)
    {
        if (entityHash.Contains(entity))
        {
            //Debug.Log("Entity Controller does not contain: " + entity);
            return;
        }
        entityHash.Remove(entity);
        masterEntityList.Remove(entity);
    }
}
