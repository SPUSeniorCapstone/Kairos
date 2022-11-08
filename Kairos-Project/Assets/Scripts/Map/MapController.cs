using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;
using TMPro;

/// <summary>
/// Not Implemented
/// </summary>
public class MapController : MonoBehaviour
{
    public static MapController main;
    public static void Init(MapController controller)
    {
        main = controller;
        PathManager.Init(new PathManager(MapController.main.grid.cellSize.x));
    }

    //GameObjects
    public RTSMap RTS;
    public MiniMap miniMap;
    public StrategicMap strategic;
    public Grid grid;
    [SerializeField] Slider scale;
    [SerializeField] Slider eccentricity;
    [SerializeField] Slider layer_divisor;


    //Other Objects
    public MapData mapData;
    public MapGenerator mapGenerator;


    public void ScaleChange()
    {
        mapGenerator.scale = scale.value;
        scale.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = scale.value.ToString();
    }

    public void EccentricityChange()
    {
        mapGenerator.eccentricity = eccentricity.value;
        eccentricity.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = eccentricity.value.ToString();
    }

    public void LayerChange()
    {
        mapGenerator.layerDivider = layer_divisor.value;
        layer_divisor.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = layer_divisor.value.ToString();
    }

    public void GenerateMap()
    {
        mapGenerator.OLD_GenerateTerrain();
    }
    private void Awake()
    {
        Init(this);
        //mapData.tiles = new MapTile[mapData.width, mapData.length];
    }

    private void Start()
    {
        mapGenerator.OLD_GenerateTerrain();
    }

    /// <summary>
    /// Not Implemented
    /// Implement with 2D map
    /// </summary>
    public Color SampleColor(Vector2 pos)
    {
        return Color.white;
    }
}
