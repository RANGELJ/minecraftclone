using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBiome {
    public static readonly RBiome DEFAULT = new RBiome(
        name: "Default",
        solidGroundHeight: 42,
        terrainHeight: 20,
        terrainScale: 0.41f,
        lodes: new RLode[]{
            new RLode(
                minHeight: 1,
                maxHeight: 41,
                scale: 0.1f,
                threshold: 0.5f,
                noiseOffset: 0,
                block: RBlock.DIRT
            ),
            new RLode(
                minHeight: 30,
                maxHeight: 40,
                scale: 0.2f,
                threshold: 0.6f,
                noiseOffset: 100,
                block: RBlock.SAND
            ),
            new RLode(
                minHeight: 10,
                maxHeight: 50,
                scale: 0.2f,
                threshold: 0.7f,
                noiseOffset: 200,
                block: RBlock.AIR
            )
        }
    );

    private string name;
    // Bellow this all will be solid
    private int solidGroundHeight;

    // Highest point from ground terraing
    private int terrainHeight;
    private float terrainScale;
    private RLode[] lodes;

    public RBiome(
        string name,
        int solidGroundHeight,
        int terrainHeight,
        float terrainScale,
        RLode[] lodes
    ) {
        this.name = name;
        this.solidGroundHeight = solidGroundHeight;
        this.terrainHeight = terrainHeight;
        this.terrainScale = terrainScale;
        this.lodes = lodes;
    }

    public int getSolidGroundHeight() {
        return this.solidGroundHeight;
    }

    public float getTerrainScale() {
        return this.terrainScale;
    }

    public int getTerrainHeight() {
        return this.terrainHeight;
    }

    public RLode[] getLodes() {
        return this.lodes;
    }
}
