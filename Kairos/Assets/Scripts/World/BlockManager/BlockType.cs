using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

/// <summary>
/// The face of the block when looking in a positive z direction 
/// </summary>
public enum FaceID
{
    FRONT, 
    BACK,
    LEFT,
    RIGHT,
    TOP,
    BOTTOM
};

/// <summary>
/// Stores the texture and color data for each type of block
/// </summary>
[Serializable]
public struct BlockType
{
    public string name;
    [Disable]
    public int id;
    public BlockTextures textures;
    public Color color;

    public BlockType(string name, int id)
    {
        this.id = id;
        this.name = name;
        this.textures = new BlockTextures();
        this.color = Color.magenta;
    }
}

/// <summary>
/// Stores the block textures as either a list of faces, or a single texture all around
/// </summary>
[Serializable]
public struct BlockTextures
{
    // When true, every face uses the same texture
    [SerializeField] bool useSingleTexture;

    // The single texture ID value
    [ConditionalHide("useSingleTexture", true, false)]
    [SerializeField] int textureID;

    // The ID's by face
    [ConditionalHide("useSingleTexture", true, true)]
    [SerializeField] int frontID, backID, leftID, rightID, topID, bottomID;


    /// <summary>
    /// Get the texture atlas ID of a blocks face
    /// </summary>
    /// <param name="face"></param>
    /// <returns></returns>
    public int GetTextureID(FaceID face)
    {
        if (useSingleTexture)
        {
            return textureID;
        }

        switch (face)
        {
            case FaceID.FRONT: return frontID;
            case FaceID.BACK: return backID;
            case FaceID.LEFT: return leftID;
            case FaceID.RIGHT: return rightID;
            case FaceID.TOP: return topID;
            case FaceID.BOTTOM: return bottomID;
            default:
                Debug.LogError("Invalid Face ID");
                return 0;
        }
    }
}
