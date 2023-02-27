using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    #region HealthBars

    private void Update()
    {
        if (GameController.Main.inputController.ToggleHealthBars.Down())
        {
            ToggleHealthBars();
        }
    }

    public enum HealthBarMode { NONE, SELECTED, ALL }
    public HealthBarMode healthBarMode;
    Dictionary<Damageable, HealthBar> healthBars = new Dictionary<Damageable, HealthBar>();
    public GameObject HealthBar;
    public void ToggleHealthBars()
    {
        if (healthBarMode == HealthBarMode.NONE)
        {
            healthBarMode = HealthBarMode.SELECTED;
        }
        else if (healthBarMode == HealthBarMode.SELECTED)
        {
            healthBarMode = HealthBarMode.ALL;
        }
        else if (healthBarMode == HealthBarMode.ALL)
        {
            healthBarMode = HealthBarMode.NONE;
        }
    }

    public void CreateHealthBar(Damageable damageble)
    {
        if (healthBars.ContainsKey(damageble))
        {
            Debug.Log("Entity has health bar");
            return;
        }
        var h = Instantiate(HealthBar, this.transform).GetComponent<HealthBar>();
        h.damageble = damageble;
        healthBars.Add(damageble, h);
    }

    public void RemoveHealthBar(Damageable damageble)
    {
        if (healthBars.ContainsKey(damageble))
        {
            var bar = healthBars[damageble];
            healthBars.Remove(damageble);
            if (bar != null)
                Destroy(bar.gameObject);
        }
    }

    #endregion
}
