using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manages Unit HealthBars
/// Press CTRL+J to show all health bars (not just selected)
/// Press CTRL+H to hide all health bars (even if selected)
/// </summary>
public class HealthBarManager : MonoBehaviour
{
    public static HealthBarManager main;

    public bool showHealthBars = true;
    public bool showOnlySelected = true;
    public GameObject HealthBar;

    Dictionary<Entity, HealthBar> healthBars = new Dictionary<Entity, HealthBar>();

    private void Awake()
    {
        if (main != null && main != this)
        {
            Debug.LogWarning("Cannot have more than one GameController in a scene");
        }
        main = this;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))        
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                ShowHealthBars(!showHealthBars);
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                showOnlySelected = !showOnlySelected;
            }
        }
    }

    public void ShowHealthBars(bool showBars)
    {
        if(showBars == showHealthBars)
        {
            return;
        }

        showHealthBars = showBars;

        foreach (var bar in healthBars)
        {
            bar.Value.gameObject.SetActive(showBars);
        }
    }

    public void CreateHealthBar(Entity entity)
    {
        var h = Instantiate(HealthBar, this.transform).GetComponent<HealthBar>();
        h.entity = entity;
        healthBars.Add(entity, h);
    }

    public void RemoveHealthBar(Entity entity)
    {
        Destroy(healthBars[entity]);
        healthBars.Remove(entity);
    }
}
