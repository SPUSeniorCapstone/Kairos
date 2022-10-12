using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public float scale = 100;
    public float eccentricity = 10;
    public bool on = false;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<Canvas>().enabled = (false);
        GenerateTerrain();

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F)) GenerateTerrain();
        if (Input.GetKeyDown(KeyCode.F)) on = !on; FindObjectOfType<Canvas>().enabled = (on); 
    }

    public void GenerateTerrain()
    {
        int resolution = Terrain.activeTerrain.terrainData.heightmapResolution;
        Debug.Log(resolution);
        float[,] tree = new float[resolution, resolution];
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                tree[x, y] = Mathf.PerlinNoise(x * scale, y * scale) * eccentricity;
            }
        }
        Terrain.activeTerrain.terrainData.SetHeightsDelayLOD(0, 0, tree);
        Terrain.activeTerrain.terrainData.SyncHeightmap();
        //TreeInstance ree = new TreeInstance();
        //Terrain.activeTerrain.AddTreeInstance(ree);
        //Vector3 pos = FindObjectOfType<Player>().transform.position;
        Vector3 pis = new Vector3(500, 60, 500);
        //pos.y = Terrain.activeTerrain.SampleHeight(FindObjectOfType<Player>().transform.position);
        GameObject.Find("Player 1").GetComponent<CharacterController>().enabled = false;
        GameObject.Find("Player 1").transform.position = pis;
        GameObject.Find("Player 1").GetComponent<CharacterController>().enabled = true;
    }
}
