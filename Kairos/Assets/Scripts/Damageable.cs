using UnityEngine;

public class Damageable : MonoBehaviour
{
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public float Health { get; private set; }
    public float HealthRatio { get { return Mathf.Clamp(Health / MaxHealth, 0, 1); } }
    [field: SerializeField] public bool Invulnerable { get; private set; }



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
    }

    private void OnDestroy()
    {
        if (GameController.Main != null && GameController.Main.UIController != null && GameController.Main.UIController.HealthBarController != null)
            GameController.Main.UIController.HealthBarController.RemoveHealthBar(this);
    }

    private void Update()
    {

        if (Health <= 0)
        {
            dead = true;
            GameController.Main.MasterDestory(this.gameObject);
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Damages the entity. Pass this function the raw damage value, and it will determine how much health to subtract from the entity
    /// <para>**Currently just removes the raw damage amount from health**</para> 
    /// </summary>
    /// <param name="damage">The raw damage value</param>
    public float Damage(float damage)
    {
        if (!Invulnerable)
        {
            Health -= damage;
            if (Health < 0)
            {
                dead = true;
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
