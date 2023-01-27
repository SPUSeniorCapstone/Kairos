using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Not Implemented
/// </summary>
public class MiniMap : MonoBehaviour
{
    public RawImage image;

    public void DrawTerrainMap()
    {
        int width = MapController.main.mapData.width;
        int length = MapController.main.mapData.length;

        Texture2D texture = new Texture2D(width, length);

        Color[] colorMap = new Color[width * length];
        for (int y = 0; y < length * MapController.main.mapData.cellSizeZ; y++)
        {
            for (int x = 0; x < width * MapController.main.mapData.cellSizeX; x++)
            {
                int index = (int)(x / MapController.main.mapData.cellSizeZ) * width + (int)(y / MapController.main.mapData.cellSizeX);
                int X = (int)(x / MapController.main.mapData.cellSizeX);
                int Y = (int)(y / MapController.main.mapData.cellSizeZ);

                colorMap[index] = MapController.main.mapData.tiles[MapController.main.mapData.GetIndex(X, Y)].mapColor;
            }
        }
        texture.SetPixels(colorMap);
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        image.texture = texture;
        //image.transform.localScale = new Vector3((float)width * MapController.main.mapData.cellSizeX / 10, 1, (float)length * MapController.main.mapData.cellSizeZ / 10);
        //image.transform.position = new Vector3((float)width * MapController.main.mapData.cellSizeX / 2, image.transform.position.y, (float)length * MapController.main.mapData.cellSizeZ / 2);
    }
}
