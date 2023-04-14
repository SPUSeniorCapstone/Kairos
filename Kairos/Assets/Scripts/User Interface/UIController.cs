using UnityEngine;

public class UIController : MonoBehaviour
{
    public Selectable currentSelected;

    public void SetUnitView()
    {

    }
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

    public StratView StratView
    {
        get
        {
            if (_stratView == null)
            {
                _stratView = FindObjectOfType<StratView>();
            }
            return (_stratView);
        }
    }
    StratView _stratView;
}
