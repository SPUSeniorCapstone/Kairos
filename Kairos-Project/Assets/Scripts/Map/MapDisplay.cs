using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;

    public void DrawTerrainMap()
    {
        int width = MapController.main.mapData.width;
        int length = MapController.main.mapData.length;

        Texture2D texture = new Texture2D(width, length);

        Color[] colorMap = new Color[width * length];
        for(int y = 0; y < length * MapController.main.mapData.cellSizeZ; y++)
        {
            for(int x = 0; x < width * MapController.main.mapData.cellSizeX; x++)
            {
                int index = (int)(y / MapController.main.mapData.cellSizeZ) * width + (int)(x / MapController.main.mapData.cellSizeX);
                int X = (int)(x / MapController.main.mapData.cellSizeX);
                int Y = (int)(y / MapController.main.mapData.cellSizeZ);

                if (MapController.main.mapData.tiles[X,Y].isPassable)
                {
                    colorMap[index] = Color.Lerp(Color.black, Color.grey, MapController.main.mapData.tiles[X, Y].weight);
                }
                else
                {
                    colorMap[index] = Color.black;
                }
            }
        }
        texture.SetPixels(colorMap);
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3((float)width * MapController.main.mapData.cellSizeX / 10, 1, (float)length * MapController.main.mapData.cellSizeZ / 10);
        textureRenderer.transform.position = new Vector3((float)width * MapController.main.mapData.cellSizeX / 2, textureRenderer.transform.position.y, (float)length * MapController.main.mapData.cellSizeZ / 2);
    }
}
