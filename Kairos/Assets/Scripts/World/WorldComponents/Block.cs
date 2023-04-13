using UnityEngine;


/// <summary>
/// Represents a 3D voxel
/// </summary>
public struct Block
{
    /// <summary>
    /// The BlockType ID number
    /// </summary>
    public int blockID;

    /// <summary>
    /// The position of the block in it's chunk
    /// </summary>
    public Vector3Int position;

    public float corruption;

    public Block(int blockID, Vector3Int position, float corruptionAmount )
    {
        this.blockID = blockID;
        this.position = position;
        this.corruption = corruptionAmount;
    }
}
