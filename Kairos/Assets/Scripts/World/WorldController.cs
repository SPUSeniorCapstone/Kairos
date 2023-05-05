using UnityEngine;

/// <summary>
/// Controller for handling world info and rendering
/// </summary>
public class WorldController : MonoBehaviour
{
    [DisableOnPlay]
    public float blockScale = 1;

    public bool loadMeshes = true;

    public bool randomSeedOnLoad = true;

    /// <summary>
    /// The Main WorldController Instance
    /// </summary>
    public static WorldController Main
    {
        get
        {
            if (main == null)
            {
                main = FindObjectOfType<WorldController>();
            }
            return main;
        }
    }
    static WorldController main;

    private void Start()
    {
        main = this;
        world = FindObjectOfType<World>();
        if (randomSeedOnLoad)
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            WorldGenerator.seed = Random.Range(int.MinValue, int.MaxValue);
        }
        GenerateWorld();
    }

    public Chunk defaultChunk;

    #region Cached Object Managers
    /// <summary>
    /// The Main World Object
    /// </summary>
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

    /// <summary>
    /// The Main WorldGenerator Object
    /// </summary>
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


    [SerializeField]
    [Button("GenerateWorld")]
    bool Button_GenerateWorld = false;
    /// <summary>
    /// Generates and populates the main World object
    /// </summary>
    public void GenerateWorld()
    {
        WorldGenerator.GenerateWorld(loadMeshes);
    }

    public Vector3Int WorldToBlockPosition(Vector3 pos)
    {
        Vector3Int position = (pos / blockScale).ToVector3Int();
        return position;
    }


    public float GetHeight(Vector3 pos)
    {
        var ipos = WorldToBlockPosition(pos);
        return world.GetHeight(ipos.x, ipos.z);
    }
}
