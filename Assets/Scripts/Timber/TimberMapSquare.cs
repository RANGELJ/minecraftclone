using System.Collections.Generic;
using UnityEngine;

public class TimberMapSquare {
    public readonly int x;
    public readonly int y;
    public readonly int z;

    public int[] vertexIndexes;

    public TimberMapSquare(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.vertexIndexes = new int[]{ -1, -1, -1, -1 };
    }

    public void assignVertexIndexIfExist(
        TimberMapSquare origin,
        int originVertex,
        int targetVertex
    ) {
        if (origin == null || this.vertexIndexes[targetVertex] != -1) {
            return;
        }
        int originVertexIndex = origin.vertexIndexes[originVertex];
        if (originVertexIndex != -1 && origin.y == this.y) {
            this.vertexIndexes[targetVertex] = originVertexIndex;
        }
    }

    override public string ToString() {
        return "(" + this.x + "," + this.y + "," + this.z + ")";
    }

    public string GetVerticesAsString(List<Vector3> vertices) {
        return "[0]" + vertices[this.vertexIndexes[0]].ToString()
            + "[1]" + vertices[this.vertexIndexes[1]].ToString()
            + "[2]" + vertices[this.vertexIndexes[2]].ToString()
            + "[3]" + vertices[this.vertexIndexes[3]].ToString();
    }
}
