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

    [SerializeField]
    [Button("GenerateWorld")]
    bool Button_GenerateWorld = false;
    public void GenerateWorld()
    {
        WorldGenerator.GenerateWorld();
    }
}
