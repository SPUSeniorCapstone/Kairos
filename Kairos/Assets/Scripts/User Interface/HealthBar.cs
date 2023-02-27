using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float defaultWidth = 80;
    public Damageable damageble;
    [SerializeField] Image healthBarImage;
    [SerializeField] Image background;

    float maxHealth;


    void Update()
    {
        if (damageble == null)
        {
            return;
        }

        if (GameController.Main.UIController.HealthBarController.healthBarMode == HealthBarController.HealthBarMode.SELECTED)
        {
            healthBarImage.gameObject.SetActive(damageble.GetComponent<Selectable>().selected);
            background.gameObject.SetActive(damageble.GetComponent<Selectable>().selected);
        }
        else if (GameController.Main.UIController.HealthBarController.healthBarMode == HealthBarController.HealthBarMode.NONE)
        {
            healthBarImage.gameObject.SetActive(false);
            background.gameObject.SetActive(false);
        }
        else
        {
            healthBarImage.gameObject.SetActive(true);
            background.gameObject.SetActive(true);
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
}
