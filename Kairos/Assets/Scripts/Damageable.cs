using UnityEngine;

public class Damageable : MonoBehaviour
{
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public float Health { get; private set; }
    public float HealthRatio { get { return Mathf.Clamp(Health / MaxHealth, 0, 1); } }
    [field: SerializeField] public bool Invulnerable { get; private set; }

    public MeshRenderer model;
    public float deathTimer = 1;
    [SerializeField]
    [Disable]
    private float DeathTime = -1f;

    public float corruptionDamageTime = 0;

    public Vector3 healthBarPosition;

    public AudioClip OnDamage;

    public AudioSource AudioSource;
    public bool Dead
    {
        get
        {
            return dead;
        }
    }
    bool dead = false;


    private void Start()
    {
        GameController.Main.UIController.HealthBarController.CreateHealthBar(this);
        model = gameObject.GetComponentInChildren<MeshRenderer>();
        AudioSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnDestroy()
    {
        //if (GameController.Main != null && GameController.Main.UIController != null && GameController.Main.UIController.HealthBarController != null)
        //GameController.Main.UIController.HealthBarController.RemoveHealthBar(this);
    }

    private void Update()
    {
        if (dead)
        {
            if (Time.time - DeathTime > deathTimer)
            {
                Destroy(this.gameObject);
            }
            else
            {
                model.material.SetFloat("_WireframeVal", 0.5f - (Time.time - DeathTime) / (deathTimer * 2));
               
            }
        }
        else
        {
            if (GameController.Main.CorruptionController.doCorruptionDamage && GetComponent<Selectable>().faction == false)
            {
                var position = WorldController.Main.WorldToBlockPosition(transform.position);
                if(WorldController.Main.World.GetCorruption(position.x,position.z) >= 1 && Time.time - corruptionDamageTime > GameController.Main.CorruptionController.damageTick)
                {
                    Damage(GameController.Main.CorruptionController.corruptionDamage * 0.01f * MaxHealth);
                    corruptionDamageTime = Time.time;
                }
            }
        }
    }

    /// <summary>
    /// Damages the entity. Pass this function the raw damage value, and it will determine how much health to subtract from the entity
    /// <para>**Currently just removes the raw damage amount from health**</para> 
    /// </summary>
    /// <param name="damage">The raw damage value</param>
    public float Damage(float damage)
    {
        if (!Invulnerable && !dead)
        {
            Health -= damage;
            if (Health <= 0)
            {
                if (GameController.Main != null && GameController.Main.UIController != null && GameController.Main.UIController.HealthBarController != null)
                {
                    GameController.Main.UIController.HealthBarController.RemoveHealthBar(this);
                }
                dead = true;        
                DeathTime = Time.time;
                model.material = GameController.Main.DeathMaterial;
                GameController.Main.MasterDestory(this.gameObject);
            }
            if (AudioSource != null)
            {
                GameController.Main.soundManager.PlayOnce(AudioSource, OnDamage);
            }
        }
        return Health;
    }

    public void Heal(float heal)
    {
        Health += heal;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

}
