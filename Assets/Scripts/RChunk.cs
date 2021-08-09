using UnityEngine;
using System.Collections.Generic;

public class RChunk {
    public static readonly byte chunkWidthInVoxels = 16;
    public static readonly byte chunkHeightInVoxels = 128;

    public Vector2Int coordinates;
    private GameObject gameObject;
    private Vector3Int position;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private List<Vector3> vertices = new List<Vector3>();
    private int trianglesCount = 0;
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    private RBlock[,,] voxelMap = new RBlock[RChunk.chunkWidthInVoxels, RChunk.chunkHeightInVoxels, RChunk.chunkWidthInVoxels];

    public delegate RBlock GetBlockByPosition(Vector3Int voxelPosition);

    private GetBlockByPosition getBlockFromPosition;

    public RChunk(
        Vector2Int coordinates,
        Material material,
        Transform parentTransform,
        GetBlockByPosition getBlockFromByPosition
    ) {
        this.getBlockFromPosition = getBlockFromByPosition;
        this.coordinates = coordinates;
        this.gameObject = new GameObject();
        this.gameObject.name = "Chunk [" + this.coordinates.x + "," + this.coordinates.y + "]";

        this.meshFilter = this.gameObject.AddComponent<MeshFilter>();

        this.meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        this.meshRenderer.material = material;

        this.gameObject.transform.SetParent(parentTransform);
        this.position = new Vector3Int(
            this.coordinates.x * RChunk.chunkWidthInVoxels,
            0,
            this.coordinates.y * RChunk.chunkWidthInVoxels
        );
        this.gameObject.transform.position = this.position;

        this.PopulateVoxelMap();

        for (int y = 0; y < RChunk.chunkHeightInVoxels; y += 1) {
            for (int x = 0; x < RChunk.chunkWidthInVoxels; x += 1) {
                for (int z = 0; z < RChunk.chunkWidthInVoxels; z += 1) {
                    this.AddVoxelMeshData(
                        position: new Vector3Int(x, y, z),
                        block: this.voxelMap[x, y, z]
                    );
                }
            }
        }

        this.CreateMesh();
    }

    public bool isActive {
        get {
            return this.gameObject.activeSelf;
        }
        set {
            this.gameObject.SetActive(value);
        }
    }

    private void PopulateVoxelMap() {
        for (int y = 0; y < RChunk.chunkHeightInVoxels; y += 1) {
            for (int x = 0; x < RChunk.chunkWidthInVoxels; x += 1) {
                for (int z = 0; z < RChunk.chunkWidthInVoxels; z += 1) {
                    this.voxelMap[x, y, z] = this.getBlockFromPosition(
                        new Vector3Int(x, y, z) + this.position
                    );
                }
            }
        }
    }

    private void AddVoxelMeshData(Vector3Int position, RBlock block) {
        if (block.Equals(RBlock.AIR)) {
            return;
        }
        for (int triangleIndex = 0; triangleIndex < RVoxel.triangles.GetLength(0); triangleIndex += 1) {
            if (this.IsVoxelSolidByPosition(position + RVoxel.faceChecks[triangleIndex])) {
                continue;
            }

            this.vertices.Add(position + RVoxel.vertices[RVoxel.triangles[triangleIndex, 0]]);
            this.vertices.Add(position + RVoxel.vertices[RVoxel.triangles[triangleIndex, 1]]);
            this.vertices.Add(position + RVoxel.vertices[RVoxel.triangles[triangleIndex, 2]]);
            this.vertices.Add(position + RVoxel.vertices[RVoxel.triangles[triangleIndex, 3]]);

            // Add texture
            this.AddTexture(block.GetTextureId(triangleIndex));

            this.triangles.Add(this.trianglesCount);
            this.triangles.Add(this.trianglesCount + 1);
            this.triangles.Add(this.trianglesCount + 2);
            this.triangles.Add(this.trianglesCount + 2);
            this.triangles.Add(this.trianglesCount + 1);
            this.triangles.Add(this.trianglesCount + 3);
            this.trianglesCount += 4;
        }
    }

    private void CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        this.meshFilter.mesh = mesh;
    }

    private bool IsVoxelInChunk(Vector3Int voxelPosition) {
        return !(voxelPosition.x < 0 || voxelPosition.x >= RChunk.chunkWidthInVoxels
            || voxelPosition.y < 0 || voxelPosition.y >= RChunk.chunkHeightInVoxels
            || voxelPosition.z < 0 || voxelPosition.z >= RChunk.chunkWidthInVoxels);
    }

    private bool IsVoxelSolidByPosition(Vector3Int voxelPosition) {
        if (!this.IsVoxelInChunk(voxelPosition)) {
            RBlock outsiderBlock = this.getBlockFromPosition(voxelPosition + this.position);
            return outsiderBlock.IsSolid();
        }
        return this.voxelMap[voxelPosition.x, voxelPosition.y, voxelPosition.z].IsSolid();
    }

    private void AddTexture(byte textureId) {
        float y = textureId / RVoxel.textureAtlasSizeInBlocks;
        float x = textureId - (y * RVoxel.textureAtlasSizeInBlocks);

        x *= RVoxel.normalizedBlockTextureSize;
        y *= RVoxel.normalizedBlockTextureSize;

        y = 1f - y - RVoxel.normalizedBlockTextureSize;

        uvs.Add(new Vector3(x, y));
        uvs.Add(new Vector3(x, y + RVoxel.normalizedBlockTextureSize));
        uvs.Add(new Vector3(x + RVoxel.normalizedBlockTextureSize, y));
        uvs.Add(new Vector3(x + RVoxel.normalizedBlockTextureSize, y + RVoxel.normalizedBlockTextureSize));
    }

    public RBlock GetBlockFromPosition(int x, int y, int z) {
        return this.voxelMap[x, y, z];
    }
}
