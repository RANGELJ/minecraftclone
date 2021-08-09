public class RLode {
    private int minHeight;
    private int maxHeight;
    private float scale;
    private float threshold;
    private float noiseOffset;
    private RBlock block;

    public RLode(
        int minHeight,
        int maxHeight,
        float scale,
        float threshold,
        float noiseOffset,
        RBlock block
    ) {
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.scale = scale;
        this.threshold = threshold;
        this.noiseOffset = noiseOffset;
        this.block = block;
    }

    public RBlock GetBlock() {
        return this.block;
    }

    public float getThreshold() {
        return this.threshold;
    }

    public int getMinHeight() {
        return this.minHeight;
    }

    public int getMaxHeight() {
        return this.maxHeight;
    }

    public float getNoiseOffset() {
        return this.noiseOffset;
    }

    public float getScale() {
        return this.scale;
    }
}
