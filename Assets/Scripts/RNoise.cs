using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNoise {
    public static float seed = 0f;

    public static float Get2DPerlin(float x, float y, float offset, float scale) {
        return Mathf.PerlinNoise(
            (x + RNoise.seed) / RChunk.chunkWidthInVoxels * scale + offset,
            (y + RNoise.seed) / RChunk.chunkWidthInVoxels * scale + offset
        );
    }

    public static bool Get3DPerlin(float x, float y, float z, float offset, float scale, float threshold) {
        float currentX = (x + offset + RNoise.seed) * scale;
        float currentY = (y + offset + RNoise.seed) * scale;
        float currentZ = (z + offset + RNoise.seed) * scale;

        float AB = Mathf.PerlinNoise(currentX, currentY);
        float BC = Mathf.PerlinNoise(currentY, currentZ);
        float AC = Mathf.PerlinNoise(currentX, currentZ);
        float BA = Mathf.PerlinNoise(currentY, currentX);
        float CB = Mathf.PerlinNoise(currentZ, currentY);
        float CA = Mathf.PerlinNoise(currentZ, currentX);

        float noise = (AB + BC + AC + BA + CB + CA) / 6f;

        if (noise > threshold) {
            return true;
        }

        return false;
    }
}
