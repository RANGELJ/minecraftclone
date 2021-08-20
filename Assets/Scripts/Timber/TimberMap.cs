using System.Collections.Generic;
using UnityEngine;

public class TimberMap {
    private GameObject gameObject;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Material meshMaterial;

    private int[] groundHeights;

    private int width;
    private int height;

    public TimberMap(
        int[] groundHeights,
        int width,
        int height,
        Material meshMaterial
    ) {
        this.groundHeights = groundHeights;
        this.width = width;
        this.height = height;
        if (this.groundHeights.Length != (this.width * this.height)) {
            throw new System.Exception("The width and height of a map should match the number of heights");
        }
        this.meshMaterial = meshMaterial;
    }

    private class MapSquare {
        public readonly int x;
        public readonly int y;
        public readonly int z;

        public int[] vertexIndexes;

        public MapSquare(int x, int y, int z) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.vertexIndexes = new int[]{ -1, -1, -1, -1 };
        }

        public void assignVertexIndexIfExist(
            MapSquare origin,
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
    }

    private MapSquare getCoordinatesFromIndex(int index, int y) {
        int z = index / this.width;
        int x = index - (z * this.width);

        return new MapSquare(x: x, y: y, z: z);
    }

    public void create() {
        this.gameObject = new GameObject();
        this.gameObject.name = "Game map";

        this.meshFilter = this.gameObject.AddComponent<MeshFilter>();
        this.meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        this.meshRenderer.material = this.meshMaterial;

        this.gameObject.transform.position = new Vector3(0f, 0f, 0f);

        List<Vector3> vertices = new List<Vector3>();
        Vector3[] extraVertices = new Vector3[this.width + 1];
        List<int> triangles = new List<int>();

        MapSquare[] squares = new MapSquare[this.groundHeights.Length];

        for (int index = 0; index < this.groundHeights.Length; index += 1) {
            int y = this.groundHeights[index];
            MapSquare currentSquare = this.getCoordinatesFromIndex(
                index: index,
                y: y
            );
            squares[index] = currentSquare;

            bool isLastInRow = currentSquare.x == this.width - 1;
            bool isFirstInRow = currentSquare.x == 0;
            bool isLastRow = currentSquare.z == this.height - 1;
            bool isFistRow = currentSquare.z == 0;

            int indexRight = isLastInRow ? -1 : index + 1;
            int indexLeft = isFirstInRow ? -1 : index - 1;
            int indexTop = isLastRow ? -1 : index + this.height;
            int indexBottom = isFistRow ? -1 : index - this.height;

            MapSquare rightNeighbor = indexRight == -1 ? null : squares[indexRight];
            MapSquare leftNeighbor = indexLeft == -1 ? null : squares[indexLeft];
            MapSquare topNeighbor = indexTop == -1 ? null : squares[indexTop];
            MapSquare bottomNeighbor = indexBottom == -1 ? null : squares[indexBottom];

            currentSquare.assignVertexIndexIfExist(
                origin: rightNeighbor,
                originVertex: 1,
                targetVertex: 0
            );
            currentSquare.assignVertexIndexIfExist(
                origin: bottomNeighbor,
                originVertex: 3,
                targetVertex: 0
            );

            if (currentSquare.vertexIndexes[0] == -1) {
                currentSquare.vertexIndexes[0] = vertices.Count;
                vertices.Add(new Vector3(currentSquare.x + 0.5f, currentSquare.y, currentSquare.z - 0.5f));
            }

            currentSquare.assignVertexIndexIfExist(
                origin: leftNeighbor,
                originVertex: 0,
                targetVertex: 1
            );

            currentSquare.assignVertexIndexIfExist(
                origin: bottomNeighbor,
                originVertex: 2,
                targetVertex: 1
            );

            if (currentSquare.vertexIndexes[1] == -1) {
                currentSquare.vertexIndexes[1] = vertices.Count;
                vertices.Add(new Vector3(currentSquare.x - 0.5f, currentSquare.y, currentSquare.z -0.5f));
            }

            currentSquare.assignVertexIndexIfExist(
                origin: leftNeighbor,
                originVertex: 3,
                targetVertex: 2
            );

            currentSquare.assignVertexIndexIfExist(
                origin: topNeighbor,
                originVertex: 1,
                targetVertex: 2
            );

            if (currentSquare.vertexIndexes[2] == -1) {
                currentSquare.vertexIndexes[2] = vertices.Count;
                vertices.Add(new Vector3(currentSquare.x - 0.5f, currentSquare.y, currentSquare.z + 0.5f));
            }

            currentSquare.assignVertexIndexIfExist(
                origin: topNeighbor,
                originVertex: 0,
                targetVertex: 3
            );

            currentSquare.assignVertexIndexIfExist(
                origin: rightNeighbor,
                originVertex: 2,
                targetVertex: 3
            );

            if (currentSquare.vertexIndexes[3] == -1) {
                currentSquare.vertexIndexes[3] = vertices.Count;
                vertices.Add(new Vector3(currentSquare.x + 0.5f, currentSquare.y, currentSquare.z + 0.5f));
            }

            // Vertices at the base of square
            triangles.Add(currentSquare.vertexIndexes[0]);
            triangles.Add(currentSquare.vertexIndexes[1]);
            triangles.Add(currentSquare.vertexIndexes[2]);

            triangles.Add(currentSquare.vertexIndexes[2]);
            triangles.Add(currentSquare.vertexIndexes[3]);
            triangles.Add(currentSquare.vertexIndexes[0]);

            if (
                bottomNeighbor != null
                && bottomNeighbor.y != currentSquare.y
                && bottomNeighbor.vertexIndexes[2] != -1
                && bottomNeighbor.vertexIndexes[3] != -1
            ) {
                // Should have a neighbor elevation
                triangles.Add(bottomNeighbor.vertexIndexes[3]);
                triangles.Add(bottomNeighbor.vertexIndexes[2]);
                triangles.Add(currentSquare.vertexIndexes[1]);

                triangles.Add(currentSquare.vertexIndexes[1]);
                triangles.Add(currentSquare.vertexIndexes[0]);
                triangles.Add(bottomNeighbor.vertexIndexes[3]);
            }

            if (
                leftNeighbor != null
                && leftNeighbor.y != currentSquare.y
                && leftNeighbor.vertexIndexes[3] != -1
                && leftNeighbor.vertexIndexes[0] != -1
            ) {
                triangles.Add(currentSquare.vertexIndexes[1]);
                triangles.Add(leftNeighbor.vertexIndexes[0]);
                triangles.Add(leftNeighbor.vertexIndexes[3]);

                triangles.Add(leftNeighbor.vertexIndexes[3]);
                triangles.Add(currentSquare.vertexIndexes[2]);
                triangles.Add(currentSquare.vertexIndexes[1]);
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        this.meshFilter.mesh = mesh;
    }
}
