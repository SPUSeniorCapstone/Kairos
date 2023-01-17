using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour, IEnumerable<Tile>
{
    [Disable]
    [SerializeField]
    int width = 10, length = 10;

    Tile[,] tiles;

    public void LoadTileMeshesFromHeightMap(float[,] heightMap)
    {

        width = heightMap.GetLength(0);
        length = heightMap.GetLength(1);
        Clear();


        for (int x = 0; x < width; x++)
        {
            for(int z = 0; z < length; z++)
            {
                float height = heightMap[x, z];

                Vector3[] vertices = new Vector3[4];
                vertices[0] = new Vector3(x, height, z);
                vertices[1] = new Vector3(x + 1, height, z);
                vertices[2] = new Vector3(x, height, z + 1);
                vertices[3] = new Vector3(x + 1, height, z + 1);

                ///tiles[x, z] = Tile.CreateTile(transform, new TileInfo(new Vector3Int(x, 0, z), height), vertices);
            }
        }
    }

    public void Clear()
    {
        if(tiles == null)
        {
            tiles = new Tile[width, length];
            return;
        }

        foreach(var tile in tiles)
        {
            if(tile != null)
            {
                DestroyImmediate(tile.gameObject);
            }
        }
        tiles = new Tile[width,length];
    }

    public int Index(int x, int z)
    {
        return GetIndex(length, x, z);
    }

    public static int GetIndex(int length, int x, int z)
    {
        return (length * z) + x;
    }

    public IEnumerator<Tile> GetEnumerator()
    {
        return tiles.GetEnumerator() as IEnumerator<Tile>;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return tiles.GetEnumerator();
    }
}
