using UnityEngine;

public class DemoChunk : MonoBehaviour {
    public float seed = 123f;
    public Material material;

    void Start() {
        RChunk chunk = new RChunk(
            coordinates: new Vector2Int(0, 0),
            material: this.material,
            parentTransform: this.transform,
            getBlockFromByPosition: (Vector3Int position) => {
                if (position.y == 0) {
                    return RBlock.ROCK;
                }

                if (position.y == (RChunk.chunkHeightInVoxels - 1)) {
                    float noise = RNoise.Get2DPerlin(
                        x: position.x + position.y + 0.1f + this.seed,
                        y: position.z + 0.1f + this.seed,
                        offset: 0f,
                        scale: 1f
                    );

                    if (noise < 0.5f) {
                        return RBlock.ROCK;
                    }
                }

                return RBlock.DIRT_WITH_GRASS;
            }
        );
    }
}
