using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

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
    [field:SerializeField] public float MaxHealth { get; private set; }
    [field:SerializeField] public float Health { get; private set; }
    public float HealthRatio { get { return Mathf.Clamp(Health / MaxHealth, 0, 1); } }
    [field: SerializeField] public bool Invulnerable { get; private set; }

    public bool isEnemy;
    public bool isPlayer;
    public bool isNeutral;
    public bool autoAttack;
    public bool deathReel;
    public Entity target;
   

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

        if (GetComponentInParent<Battalion>() && GetComponentInParent<Battalion>().isEnemy)
        {
            isEnemy = true;
        }
        if (GetComponentInParent<Battalion>() && GetComponentInParent<Battalion>().autoAttack)
        {
            autoAttack = true;
        }
        if (GetComponentInParent<Battalion>() && GetComponentInParent<Battalion>().isPlayer)
        {
            isPlayer = true;
        }
        //!DELETE
        GameController.main.playerController.playerEntities.Add(this);

        EntityController.main.RegisterEntity(this);
      
    }

    private void OnDestroy()
    {
        Debug.Log("I'm called!");
        GameController.main.playerController.playerEntities.Remove(this);
        GameController.main.playerController.selectedEntityList.Remove(this);
        EntityController.main.UnregisterEntity(this);
        
    }

    /// <summary>
    /// Damages the entity. Pass this function the raw damage value, and it will determine how much health to subtract from the entity
    /// <para>**Currently just removes the raw damage amount from health**</para> 
    /// </summary>
    /// <param name="damage">The raw damage value</param>
    public void DamageEntity(float damage)
    {
        if (!Invulnerable)
            Health -= damage;
    }

    public void Update()
    {
        if (deathReel)
        {
            transform.position -= Vector3.up * 1f * Time.deltaTime;
        }
        if (transform.position.y == -5)
        {
            Destroy(this);
        }
    }

    public void DestroyEntity()
    {
        deathReel = true;
        //GameController.main.playerController.playerEntities.Remove(this);
        //EntityController.main.UnregisterEntity(this);
        Destroy(gameObject);
    }

    private void OnMouseOver()
    {
        if (GameController.main.capture == null)
        {
            Debug.Log("Unsuccesful");
        }
        if (isEnemy)
        {
            GameController.main.playerController.onEnemy = true;
            if (GetComponentInParent<Battalion>())
            {
                GameController.main.playerController.enemyPos = GetComponentInParent<Battalion>().GetCenter();
            }
            else
            {
                GameController.main.playerController.enemyPos = transform.position;
            }
            //Debug.Log("True enemy, pos" + GameController.main.playerController.enemyPos);
            Cursor.SetCursor(GameController.main.enemy, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(GameController.main.capture, Vector2.zero, CursorMode.Auto);
            //Debug.Log("The mouse sees me");
            //Debug.Log(GameController.main.capture);
        }
      
      
    }
    private void OnMouseExit()
    {
        GameController.main.playerController.onEnemy = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
       // Debug.Log("off");
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
