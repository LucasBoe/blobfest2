using System.Collections.Generic;
using UnityEngine;

namespace VoronoiMap
{
    public class VoronoiMeshCreator : MonoBehaviour
    {
        [SerializeField] Material material;
        private GameObject lastMesh;
        public void GenerateMesh(VoronoiMapData map)
        {
            var mapGo = new GameObject("==MAP==");
            for (int i = 0; i < map.Cells.Count; i++)
            {
                VoronoiCellData cell = map.Cells[i];
                GameObject go = new GameObject($"CELL{i}");
                go.transform.parent = mapGo.transform;
                go.transform.position = (cell.Center);

                var renderer = go.AddComponent<MeshRenderer>();
                var filter = go.AddComponent<MeshFilter>();

                var points = ToLocalV2Space(cell.Edges, cell.Center);

                Mesh mesh = CreatePolygonMeshAroundZeroPos(points);

                filter.sharedMesh = mesh;

                var mat = new Material(material);
                mat.color = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1, 1);
                renderer.material = mat;
            }

            lastMesh = mapGo;
        }

        public static Mesh CreatePolygonMeshAroundZeroPos(List<Vector3> points)
        {
            Mesh mesh = new Mesh();
            List<Vector3> newVertices = new List<Vector3>();
            List<Vector3> newNormals = new List<Vector3>();
            List<Vector2> newUV = new List<Vector2>();
            List<int> newTriangles = new List<int>();

            newVertices.Add(Vector3.zero);
            newUV.Add(new Vector2(0, 0.5f));
            newNormals.Add(Vector3.up);

            for (int j = 0; j <= points.Count; j++)
            {
                int index = j;

                if (index < points.Count)
                {
                    newVertices.Add(new Vector3(points[j].x, points[j].y, points[j].z));
                    newUV.Add(new Vector2(1f, (j / (float)points.Count) * 3f - 1f));
                    newNormals.Add(Vector3.up);
                }

                if (index > 1)
                {
                    newTriangles.Add(0);
                    newTriangles.Add(index - 1);
                    newTriangles.Add(index);
                }
            }

            newTriangles.Add(0);
            newTriangles.Add(points.Count);
            newTriangles.Add(1);

            mesh.vertices = newVertices.ToArray();
            mesh.triangles = newTriangles.ToArray();
            mesh.uv = newUV.ToArray();
            mesh.normals = newNormals.ToArray();
            mesh.RecalculateNormals();
            mesh.Optimize();
            return mesh;
        }

        private List<Vector3> ToLocalV2Space(Vector2[] edges, Vector2 center)
        {
            List<Vector3> corrected = new List<Vector3>();

            foreach (var point in edges)
                corrected.Add((point - center));

            return corrected;
        }
        internal void DeleteLastMesh() => Destroy(lastMesh);
    }
}