using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class NewGameMenu : MonoBehaviour
{
    public WorldGenerator WorldGenerator;

    public VisualElement MapView;

    UIDocument document;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        if (document != null)
            MapView = document.rootVisualElement.Q("Map");
    }

    private void Start()
    {
        MapView.style.backgroundImage = WorldGenerator.GenerateWorldTexture();
    }
}
