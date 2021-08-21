using System.Collections.Generic;
using UnityEngine;

public class TimberMap {
    private GameObject gameObject;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Material meshMaterial;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private TimberMapSquare[] squares;

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
        this.squares = new TimberMapSquare[this.groundHeights.Length];
    }

    private TimberMapSquare getCoordinatesFromIndex(int index, int y) {
        int z = index / this.width;
        int x = index - (z * this.width);

        return new TimberMapSquare(x: x, y: y, z: z);
    }

    public void computeVerticesAndTriangles() {
        this.gameObject = new GameObject();
        this.gameObject.name = "Game map";

        this.meshFilter = this.gameObject.AddComponent<MeshFilter>();
        this.meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        this.meshRenderer.material = this.meshMaterial;

        this.gameObject.transform.position = new Vector3(0f, 0f, 0f);

        for (int squareIndex = 0; squareIndex < this.groundHeights.Length; squareIndex += 1) {
            int y = this.groundHeights[squareIndex];
            TimberMapSquare currentSquare = this.getCoordinatesFromIndex(
                index: squareIndex,
                y: y
            );
            squares[squareIndex] = currentSquare;

            bool isLastInRow = currentSquare.x == this.width - 1;
            bool isFirstInRow = currentSquare.x == 0;
            bool isLastRow = currentSquare.z == this.height - 1;
            bool isFistRow = currentSquare.z == 0;

            int indexRight = isLastInRow ? -1 : squareIndex + 1; // 4
            int indexLeft = isFirstInRow ? -1 : squareIndex - 1; // -1
            int indexTop = isLastRow ? -1 : squareIndex + this.height; // -1
            int indexBottom = isFistRow ? -1 : squareIndex - this.width; // 0

            TimberMapSquare rightNeighbor = indexRight == -1 ? null : squares[indexRight];
            TimberMapSquare leftNeighbor = indexLeft == -1 ? null : squares[indexLeft];
            TimberMapSquare topNeighbor = indexTop == -1 ? null : squares[indexTop];
            TimberMapSquare bottomNeighbor = indexBottom == -1 ? null : squares[indexBottom];

            if (squareIndex == 3) {
                Debug.Log("indexBottom: " + indexBottom);
            }

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

            if (squareIndex == 3) {
                Debug.Log("Computed vertices " + currentSquare.GetVerticesAsString(this.vertices));
            }

            // Vertices at the base of square
            triangles.Add(currentSquare.vertexIndexes[0]);
            triangles.Add(currentSquare.vertexIndexes[1]);
            triangles.Add(currentSquare.vertexIndexes[2]);

            triangles.Add(currentSquare.vertexIndexes[2]);
            triangles.Add(currentSquare.vertexIndexes[3]);
            triangles.Add(currentSquare.vertexIndexes[0]);

            // Vertices to connect diferent y squares
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
    }

    public IEnumerator<WaitForSeconds> BuildDebugMesh() {
        for (int squareIndex = 0; squareIndex < this.squares.Length; squareIndex += 1) {
            Debug.Log("Square index: " + squareIndex);
            TimberMapSquare square = this.squares[squareIndex];

            int vertexIndex1 = square.vertexIndexes[0];
            int vertexIndex2 = square.vertexIndexes[1];
            int vertexIndex3 = square.vertexIndexes[2];

            Debug.DrawLine(
                this.vertices[vertexIndex1],
                this.vertices[vertexIndex2],
                Color.white,
                300f
            );
            Debug.DrawLine(
                this.vertices[vertexIndex2],
                this.vertices[vertexIndex3],
                Color.white,
                300f
            );

            yield return new WaitForSeconds(2);
        }
    }

    public void CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        this.meshFilter.mesh = mesh;
    }
}
