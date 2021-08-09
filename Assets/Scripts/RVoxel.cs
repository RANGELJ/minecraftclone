using UnityEngine;

public class RVoxel {
    public static readonly Vector3[] vertices = {
        Vector3.zero,
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 1.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 1.0f)
    };

    public static readonly Vector3Int[] faceChecks = {
        new Vector3Int(0, 0, -1), // Back
        new Vector3Int(0, 0, 1), // Front
        new Vector3Int(0, 1, 0), // Top
        new Vector3Int(0, -1, 0), // Bottom
        new Vector3Int(-1, 0, 0), // Left
        new Vector3Int(1, 0, 0) // Right
    };

    public static readonly int[,] triangles = {
        // Back, Front, Top, Bottom, Left, Right
        { 0, 3, 1, 2 }, // Back
        { 5, 6, 4, 7 }, // Front
        { 3, 7, 2, 6 }, // Top
        { 1, 5, 0, 4 }, // Bottom
        { 4, 7, 0, 3 }, // Left
        { 1, 2, 5, 6 } // Right
    };

    public static readonly Vector2[] uvs = {
        new Vector2(0.0f, 0.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(1.0f, 1.0f)
    };

    public static readonly int textureAtlasSizeInBlocks = 4;

    public static float normalizedBlockTextureSize {
        get {
            return 1f / (float)textureAtlasSizeInBlocks;
        }
    }
}
