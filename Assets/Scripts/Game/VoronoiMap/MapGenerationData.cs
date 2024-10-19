using System.Collections.Generic;
using UnityEngine;

namespace VoronoiMap
{
    public class MapGenerationData
    {
        public Vector2 Size;
        public List<uint> Colors = new();
        public List<Vector2> Points = new();

        public MapGenerationData(int pointCount, Vector2 size)
        {
            for (int i = 0; i < pointCount; i++)
            {
                Colors.Add(0);
                Points.Add(new Vector2(
                        UnityEngine.Random.Range(0, size.x),
                        UnityEngine.Random.Range(0, size.y))
                );
            }

            Size = size;
        }
    }
}
