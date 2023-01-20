using UnityEngine;

public class WorldController : MonoBehaviour
{
    public static WorldController main
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<WorldController>();
            }
            return instance;
        }
    }
    static WorldController instance;

    public Chunk defaultChunk;

    #region Cached Object Managers
    public BiomeManager BiomeManager
    {
        get
        {
            if(biomeManager == null)
            {
                biomeManager = FindObjectOfType<BiomeManager>();
            }
            return biomeManager;
        }
    }
    BiomeManager biomeManager;

    public TileManager TileManager
    {
        get
        {
            if (tileManager == null)
            {
                tileManager = FindObjectOfType<TileManager>();
            }
            return tileManager;
        }
    }
    TileManager tileManager;

    public World World
    {
        get
        {
            if (world == null)
            {
                world = FindObjectOfType<World>();
            }
            return world;
        }
    }
    World world;

    public WorldGenerator WorldGenerator
    {
        get
        {
            if (worldGenerator == null)
            {
                worldGenerator = FindObjectOfType<WorldGenerator>();
            }
            return worldGenerator;
        }
    }
    WorldGenerator worldGenerator;

    #endregion

    private void Start()
    {
        string filename = "TextureAtlas.png";

        TileManager.GenerateTextureAtlas();
        TileManager.SaveTextureAtlasToFile(filename);
        Texture tex = Resources.Load<Texture>(filename);
        Debug.Log(tex);
        World.material.SetTexture("_MainTex", tex);
        GenerateWorld();
    }

    [SerializeField]
    [Button("GenerateWorld")]
    bool Button_GenerateWorld = false;
    public void GenerateWorld()
    {
        WorldGenerator.GenerateWorld(World);
    }
}
