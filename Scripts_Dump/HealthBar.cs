using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float defaultWidth = 80;
    public Entity entity;
    [SerializeField] Image healthBarImage;
    [SerializeField] Image background;

    float maxHealth;


    void Update()
    {
        if (entity == null)
        {
            return;
        }

        if (HealthBarManager.main.showOnlySelected)
        {
            healthBarImage.gameObject.SetActive(entity.select);
            background.gameObject.SetActive(entity.select);
        }
        else
        {
            healthBarImage.gameObject.SetActive(true);
            background.gameObject.SetActive(true);
        }

        if(maxHealth != entity.MaxHealth)
        {
            maxHealth = entity.MaxHealth;

            float width = defaultWidth + (maxHealth / 30);

            healthBarImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            background.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width + 10);
        }

        if (entity.select || !HealthBarManager.main.showOnlySelected)
        {
            var pos = Camera.main.WorldToScreenPoint(entity.transform.position + entity.HealthBarPosition);
            transform.position = pos;

            float fillamount = entity.Health / entity.MaxHealth;

            healthBarImage.fillAmount = fillamount;
            healthBarImage.color = Color.Lerp(Color.red, Color.green, fillamount);
        }
    }
}
