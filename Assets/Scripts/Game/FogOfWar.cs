using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FogOfWar : Engine.SingletonBehaviour<FogOfWar>
{
    public List<Cell> cells;
    public List<Vector2[]> cellShapes; // Each cell is defined by an array of Vector2 points.
    private Mesh fogMesh;
    private Color[] vertexColors;

    public void GenerateFogMesh(List<Cell> cells)
    {
        this.cells = cells;
        this.cellShapes = cells.Select(c => c.Edges).ToList();
        fogMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Color> colors = new List<Color>();

        int vertexIndex = 0;

        // Iterate through each cell to define vertices and triangles
        foreach (var cell in cellShapes)
        {
            int startIndex = vertexIndex;

            // Add each vertex to the vertices list, convert Vector2 to Vector3
            foreach (var point in cell)
            {
                vertices.Add(new Vector3(point.x, point.y, 0));
                colors.Add(Color.black); // Initial color is fully opaque (black).
                vertexIndex++;
            }

            // Define triangles for each cell (assumes each cell is convex and can be triangulated as a fan from the first vertex)
            for (int i = 1; i < cell.Length - 1; i++)
            {
                triangles.Add(startIndex);
                triangles.Add(startIndex + i);
                triangles.Add(startIndex + i + 1);
            }
        }

        fogMesh.vertices = vertices.ToArray();
        fogMesh.triangles = triangles.ToArray();
        fogMesh.colors = colors.ToArray(); // Assign initial colors to vertices

        GetComponent<MeshFilter>().mesh = fogMesh;

        // Save the colors array for updating visibility
        vertexColors = colors.ToArray();
    }

    public void SetCellVisibility(Cell cell, bool isVisible)
    {
        SetCellVisibility(cells.IndexOf(cell), isVisible);
    }

    // Method to set visibility of a cell based on its index
    public void SetCellVisibility(int cellIndex, bool isVisible)
    {
        if (cellIndex < 0 || cellIndex >= cellShapes.Count)
            return;

        // Get the start and length for this cell’s vertices in the vertexColors array
        int vertexStartIndex = GetVertexStartIndex(cellIndex);
        int vertexCount = cellShapes[cellIndex].Length;

        // Update vertex colors based on visibility
        Color targetColor = isVisible ? new Color(0, 0, 0, 0) : Color.black;
        for (int i = vertexStartIndex; i < vertexStartIndex + vertexCount; i++)
        {
            vertexColors[i] = targetColor;
        }

        // Apply updated colors to the mesh
        fogMesh.colors = vertexColors;
    }

    // Helper to find where each cell starts in the vertex array
    private int GetVertexStartIndex(int cellIndex)
    {
        int index = 0;
        for (int i = 0; i < cellIndex; i++)
        {
            index += cellShapes[i].Length;
        }
        return index;
    }
}