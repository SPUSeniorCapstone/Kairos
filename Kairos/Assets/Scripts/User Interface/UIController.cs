using UnityEngine;

public class UIController : MonoBehaviour
{
    public HealthBarController HealthBarController
    {
        get
        {
            if (_healthBarController == null)
            {
                _healthBarController = FindObjectOfType<HealthBarController>();
            }
            return _healthBarController;
        }
    }
    HealthBarController _healthBarController;

    public MenuController MenuController
    {
        get
        {
            if (_menuController == null)
            {
                _menuController = FindObjectOfType<MenuController>();
            }
            return _menuController;
        }
    }
    MenuController _menuController;
}
