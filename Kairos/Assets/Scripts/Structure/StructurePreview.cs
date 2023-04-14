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



    private void Update()
    {
       
        
           if (!example)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
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
            }
        }
        
    }
    public void Start()
    {
      
        base.Start();
        damageable = GetComponent<Damageable>();
    }

   
    public override void OnSelect()
    {
        Debug.Log("OnSelect");
  
    }
    public override void OnDeselect()
    {
        Debug.Log("off");
       
      
    }
}
