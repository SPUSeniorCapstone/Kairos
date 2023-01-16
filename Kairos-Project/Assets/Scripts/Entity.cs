using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class from which all units and structures inherit
/// </summary>
public abstract class Entity : MonoBehaviour
{
    [SerializeField]
    GameObject selectedHighlight;
    public bool select = false;
    public KeyCode hotkey;
    public float rotateSpeed = 10f;
    public Vector3 HealthBarPosition;

    //Health
    [field:SerializeField] public abstract float MaxHealth { get; protected set; }
    [field:SerializeField] public abstract float Health { get; protected set; }
    public float HealthRatio { get { return Mathf.Clamp(Health / MaxHealth, 0, 1); } }
    [field: SerializeField] public bool Invulnerable { get; private set; }

    public void SetSelectedVisible(bool selected)
    {
        if(selectedHighlight != null)
        {
            selectedHighlight.SetActive(selected);
            select = selected;
        }
    }

    protected void Start()
    {
        //!DELETE
        GameController.main.playerController.playerEntities.Add(gameObject);

        EntityController.main.RegisterEntity(this);
    }

    private void OnDestroy()
    {
        GameController.main.playerController.playerEntities.Remove(gameObject);
    }

    /// <summary>
    /// Damages the entity. Pass this function the raw damage value, and it will determine how much health to subtract from the entity
    /// <para>**Currently just removes the raw damage amount from health**</para> 
    /// </summary>
    /// <param name="damage">The raw damage value</param>
    public void DamageEntity(float damage)
    {
        if(!Invulnerable)
            Health -= damage;
    }

    protected void RotateTowards(Vector3 pos)
    {

        pos.y = transform.position.y;
        Quaternion rotation = transform.rotation;

        Vector3 direction = pos - transform.position;
        var lookRotation = Quaternion.LookRotation(direction);


        rotation = Quaternion.Slerp(rotation, lookRotation, Time.deltaTime * rotateSpeed);
        transform.rotation = rotation;
    }
}
