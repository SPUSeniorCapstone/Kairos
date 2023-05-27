using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float defaultWidth = 80;
    public Damageable damageble;
    [SerializeField] Image healthBarImage;
    [SerializeField] Image background;
    public Image icon;
    public bool UseIcon = false;

    float maxHealth;

    private void Start()
    {
        if(damageble.Icon != null)
        {
            icon.sprite = damageble.Icon;
            icon.gameObject.SetActive(true);
            UseIcon = true;
        }
        else
        {
            UseIcon = false;
        }
    }

    void Update()
    {
        if (damageble == null)
        {
            return;
        }

        if (GameController.Main.UIController.HealthBarController.healthBarMode == HealthBarController.HealthBarMode.SELECTED)
        {
            EnableBar(damageble.GetComponent<Selectable>().selected);
        }
        else if (GameController.Main.UIController.HealthBarController.healthBarMode == HealthBarController.HealthBarMode.NONE)
        {
            EnableBar(false);
        }
        else if (GameController.Main.UIController.HealthBarController.healthBarMode == HealthBarController.HealthBarMode.DAMAGED)
        {
            if (damageble.Health < damageble.MaxHealth)
            {
                EnableBar(true, damageble.GetComponent<Selectable>().selected);
                
            }
            else
            {
                EnableBar(damageble.GetComponent<Selectable>().selected);
            }
        }
        else
        {
            EnableBar(true, damageble.GetComponent<Selectable>().selected);
        }

        if (maxHealth != damageble.MaxHealth)
        {
            maxHealth = damageble.MaxHealth;

            float width = defaultWidth + (maxHealth / 30);

            healthBarImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            background.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width + 10);
        }

        if (GameController.Main.UIController.HealthBarController.healthBarMode != HealthBarController.HealthBarMode.SELECTED || (damageble.GetComponent<Selectable>() != null && damageble.GetComponent<Selectable>().selected))
        {
            var pos = Camera.main.WorldToScreenPoint(damageble.transform.position + damageble.healthBarPosition);
            transform.position = pos;

            float fillamount = damageble.Health / damageble.MaxHealth;

            healthBarImage.fillAmount = fillamount;
            healthBarImage.color = Color.Lerp(Color.red, Color.green, fillamount);
        }
    }

    public void EnableBar(bool enabled, bool changeicon = true)
    {
        healthBarImage.gameObject.SetActive(enabled);
        background.gameObject.SetActive(enabled);
        if(UseIcon)
            icon.gameObject.SetActive(enabled && damageble.GetComponent<Selectable>().selected);
    }
}
