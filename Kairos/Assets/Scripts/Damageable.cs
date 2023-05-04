using UnityEngine;

public class Damageable : MonoBehaviour
{
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public float Health { get; private set; }
    public float HealthRatio { get { return Mathf.Clamp(Health / MaxHealth, 0, 1); } }
    [field: SerializeField] public bool Invulnerable { get; private set; }

    private MeshRenderer model;
    public float deathTimer = 1;
    [SerializeField]
    [Disable]
    private float DeathTime = -1f;



    public Vector3 healthBarPosition;
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
                dead = true;
                if (GameController.Main != null && GameController.Main.UIController != null && GameController.Main.UIController.HealthBarController != null)
                {
                    GameController.Main.UIController.HealthBarController.RemoveHealthBar(this);
                }
                DeathTime = Time.time;
                model.material = GameController.Main.DeathMaterial;
                GameController.Main.MasterDestory(this.gameObject);
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
