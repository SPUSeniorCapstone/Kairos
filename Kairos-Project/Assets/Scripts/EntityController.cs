using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EntityController : MonoBehaviour
{
    public static EntityController main;
    private void Awake()
    {
        if (main != null && main != this)
        {
            Debug.LogWarning("Cannot have more than one EntityController in a scene");
        }
        main = this;
    }

    [SerializeField]  List<Entity> entities = new List<Entity>();
    
   public List<Entity> Entities { get { return entities; } }

    public void RegisterEntity(Entity entity)
    {
        entities.Add(entity);
        HealthBarManager.main.CreateHealthBar(entity);
    }

    public void UnregisterEntity(Entity entity)
    {
        entities.Remove(entity);
        HealthBarManager.main.RemoveHealthBar(entity);
    }

    public void CreateEntityHealthBar(Entity entity)
    {

    }
}
