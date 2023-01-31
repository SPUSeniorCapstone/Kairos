using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for handling world info and rendering
/// </summary>
public class WorldController : MonoBehaviour
{
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
        WorldGenerator.GenerateWorld();
    }
}
