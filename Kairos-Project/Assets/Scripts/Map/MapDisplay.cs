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
        for(int y = 0; y < length; y++)
        {
            for(int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.blue, Color.green, MapController.main.mapData.tiles[x, y].height);
            }
        }
        texture.SetPixels(colorMap);
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(width / 10, 1, length / 10);
    }
}
