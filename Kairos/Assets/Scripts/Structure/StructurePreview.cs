using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class StructurePreview : Structure
{

    protected float timeLeft = 1;
    protected float originialTime = 1;
    protected Damageable damageable;
    public Structure structure;
    public float distanceToBuild;



    private void Update()
    {
        foreach (Builder_Unit b in GameController.Main.masterBuilder)
        {
            if (builder == null)
            {
                builder = b;
            } 
            else if (Vector3.Distance(transform.position, builder.transform.position) > Vector3.Distance(transform.position, b.transform.position))
            {
                builder = b;
            }
        }
       
        
           if (!example)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0 && Vector3.Distance(transform.position, builder.transform.position) < distanceToBuild && builder.entity.movementMode == Builder_Entity.MovementMode.IDLE)
            {
                timeLeft = originialTime;
                damageable.Heal(100);
            }
            if (damageable.Health == damageable.MaxHealth)
            {
                Debug.Log("Full health!");
                var s = Instantiate<Structure>(structure, directory.transform);
                s.transform.position = transform.position;
                s.Preview = gameObject;
                damageable.Damage(damageable.MaxHealth);
                //Destroy(gameObject);
            }
        }
        
    }
    new public void Start()
    {
      
        base.Start();
        damageable = GetComponent<Damageable>();
        damageable.deathTimer = 0;
        distanceToBuild = Size.x * 6 + 2;
    }

   
    public override void OnSelect()
    {
        
    }
    public override void OnDeselect()
    {
       
      
    }
}
