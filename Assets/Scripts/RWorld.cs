using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RWorld : MonoBehaviour {
    private static readonly int sizeInChunks = 100;
    private static readonly byte viewDistanceInChunks = 2;
    private RChunk[,] chunks = new RChunk[RWorld.sizeInChunks, RWorld.sizeInChunks];
    private List<RChunk> activeChunks = new List<RChunk>();
    private Vector3 lastPlayerPosition;

    public int seed = 123;
    public Material chunkMaterial;
    public Transform playerTransform;

    void Start() {
        RNoise.seed = this.seed;

        float xSpawnPosition = (RWorld.sizeInChunks * RChunk.chunkWidthInVoxels) / 2f;

        Vector3 spawnPosition = new Vector3(
            xSpawnPosition,
            RChunk.chunkHeightInVoxels - 50f,
            xSpawnPosition
        );

        this.playerTransform.position = spawnPosition;

        this.ReCheckViewDistance();
        this.lastPlayerPosition = this.playerTransform.position;
    }

    void Update() {
        Vector3 currentPlayerPosition = this.playerTransform.transform.position;

        if (!currentPlayerPosition.Equals(this.lastPlayerPosition)) {
            this.ReCheckViewDistance();
            this.lastPlayerPosition = currentPlayerPosition;
        }
    }

    private void ReCheckViewDistance() {
        Vector3 currentPlayerPosition = this.playerTransform.transform.position;
        int playerXChunkPos = Mathf.FloorToInt(currentPlayerPosition.x / RChunk.chunkWidthInVoxels);
        int playerZChunkPos = Mathf.FloorToInt(currentPlayerPosition.z / RChunk.chunkWidthInVoxels);

        List<RChunk> previusActiveChunks = new List<RChunk>(this.activeChunks);
        this.activeChunks = new List<RChunk>();

        for (
            int x = playerXChunkPos - RWorld.viewDistanceInChunks;
            x < playerXChunkPos + RWorld.viewDistanceInChunks;
            x += 1
        ) {
            for (
                int z = playerZChunkPos - RWorld.viewDistanceInChunks;
                z <= playerZChunkPos + RWorld.viewDistanceInChunks;
                z += 1
            ) {
                if (!this.IsChunkCoordinateInWorld(x: x, z: z)) {
                    continue;
                }

                if (this.chunks[x, z] == null) {
                    this.CreateNewChunk(x, z);
                } else {
                    this.chunks[x, z].isActive = true;
                    this.activeChunks.Add(this.chunks[x, z]);
                }

                previusActiveChunks = RUtils.FilterList(previusActiveChunks, (RChunk prevActiveChunk, int chunkIndex) => {
                    bool areEqual = prevActiveChunk.coordinates.x == x
                        && prevActiveChunk.coordinates.y == z;

                    return !areEqual;
                });
            }
        }

        previusActiveChunks.ForEach((RChunk chunk) => {
            chunk.isActive = false;
        });
    }

    public RBlock GetBlockFromPosition(float x, float y, float z) {
        int xCheck = Mathf.FloorToInt(x);
        int yCheck = Mathf.FloorToInt(y);
        int zCheck = Mathf.FloorToInt(z);

        int xChunk = xCheck / RChunk.chunkWidthInVoxels;
        int zChunk = zCheck / RChunk.chunkWidthInVoxels;

        xCheck -= xChunk * RChunk.chunkWidthInVoxels;
        zCheck -= zChunk * RChunk.chunkWidthInVoxels;

        RChunk chunk = this.chunks[xChunk, zChunk];

        RBlock block = chunk.GetBlockFromPosition(
            x: xCheck,
            y: yCheck,
            z: zCheck
        );

        return block;
    }

    private RBlock ComputeBlockFromPostion(Vector3Int position) {
        // First check
        if (!this.IsVoxelInWorld(position)) {
            return RBlock.AIR;
        }

        if (position.y == 0) {
            return RBlock.BED_ROCK;
        }

        int terrainHeight = Mathf.FloorToInt(RBiome.DEFAULT.getTerrainHeight() * RNoise.Get2DPerlin(
            x: position.x,
            y: position.z,
            offset: 0f,
            scale: RBiome.DEFAULT.getTerrainScale()
        )) + RBiome.DEFAULT.getSolidGroundHeight();

        if (position.y > terrainHeight) {
            return RBlock.AIR;
        }

        RBlock pickedBlock;

        if (position.y == terrainHeight) {
            pickedBlock = RBlock.DIRT_WITH_GRASS;
        } else if (position.y > terrainHeight - 5) {
            pickedBlock = RBlock.DIRT;
        } else {
            pickedBlock = RBlock.ROCK;
        }

        // Second check
        foreach (RLode lode in RBiome.DEFAULT.getLodes()) {
            if (
                position.y > lode.getMinHeight()
                && position.y < lode.getMaxHeight()
            ) {
                bool pass = RNoise.Get3DPerlin(
                    x: position.x,
                    y: position.y,
                    z: position.z,
                    offset: lode.getNoiseOffset(),
                    scale: lode.getScale(),
                    threshold: lode.getThreshold()
                );

                if (pass) {
                    pickedBlock = lode.GetBlock();
                    break;
                }
            }
        }

        return pickedBlock;
    }

    private bool IsChunkCoordinateInWorld(int x, int z) {
        return x >= 0
            && x < RWorld.sizeInChunks
            && z >= 0
            && z < RWorld.sizeInChunks;
    }

    private bool IsVoxelInWorld(Vector3Int position) {
        return position.x >= 0
            && position.x < (RWorld.sizeInChunks * RChunk.chunkWidthInVoxels)
            && position.y >= 0
            && position.y < RChunk.chunkHeightInVoxels
            && position.z >= 0
            && position.z < (RWorld.sizeInChunks * RChunk.chunkWidthInVoxels);
    }

    private void CreateNewChunk(int x, int z) {
        this.chunks[x, z] = new RChunk(
            coordinates: new Vector2Int(x, z),
            material: this.chunkMaterial,
            parentTransform: this.transform,
            getBlockFromByPosition: this.ComputeBlockFromPostion
        );
        this.activeChunks.Add(this.chunks[x, z]);
    }
}
