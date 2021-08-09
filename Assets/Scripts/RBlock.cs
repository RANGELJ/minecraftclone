using System;

public class RBlock {
    public static readonly RBlock AIR = new RBlock(
        isSolid: false,
        backFaceTexture: 1,
        frontFaceTexture: 1,
        topFaceTexture: 1,
        bottomFaceTexture: 1,
        leftFaceTexture: 1,
        rightFaceTexture: 1
    );

    public static readonly RBlock DIRT_WITH_GRASS = new RBlock(
        isSolid: true,
        backFaceTexture: 2,
        frontFaceTexture: 2,
        topFaceTexture: 7,
        bottomFaceTexture: 1,
        leftFaceTexture: 2,
        rightFaceTexture: 2
    );

    public static readonly RBlock DIRT = new RBlock(
        isSolid: true,
        backFaceTexture: 1,
        frontFaceTexture: 1,
        topFaceTexture: 1,
        bottomFaceTexture: 1,
        leftFaceTexture: 1,
        rightFaceTexture: 1
    );

    public static readonly RBlock ROCK = new RBlock(
        isSolid: true,
        backFaceTexture: 8,
        frontFaceTexture: 8,
        topFaceTexture: 8,
        bottomFaceTexture: 8,
        leftFaceTexture: 8,
        rightFaceTexture: 8
    );

    public static readonly RBlock SAND = new RBlock(
        isSolid: true,
        backFaceTexture: 10,
        frontFaceTexture: 10,
        topFaceTexture: 10,
        bottomFaceTexture: 10,
        leftFaceTexture: 10,
        rightFaceTexture: 10
    );

    public static readonly RBlock BED_ROCK = new RBlock(
        isSolid: true,
        backFaceTexture: 9,
        frontFaceTexture: 9,
        topFaceTexture: 9,
        bottomFaceTexture: 9,
        leftFaceTexture: 9,
        rightFaceTexture: 9
    );

    private bool isSolid;
    private byte backFaceTexture;
    private byte frontFaceTexture;
    private byte topFaceTexture;
    private byte bottomFaceTexture;
    private byte leftFaceTexture;
    private byte rightFaceTexture;

    private RBlock(
        bool isSolid,
        byte backFaceTexture,
        byte frontFaceTexture,
        byte topFaceTexture,
        byte bottomFaceTexture,
        byte leftFaceTexture,
        byte rightFaceTexture
    ) {
        this.isSolid = isSolid;
        this.backFaceTexture = backFaceTexture;
        this.frontFaceTexture = frontFaceTexture;
        this.topFaceTexture = topFaceTexture;
        this.bottomFaceTexture = bottomFaceTexture;
        this.leftFaceTexture = leftFaceTexture;
        this.rightFaceTexture = rightFaceTexture;
    }

    public bool IsSolid() {
        return this.isSolid;
    }

    public byte GetTextureId(int faceIndex) {
        switch (faceIndex) {
            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                throw new Exception("Invalid face index: " + faceIndex);
        }
    }
}
