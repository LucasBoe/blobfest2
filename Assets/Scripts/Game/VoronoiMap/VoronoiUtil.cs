using Delaunay;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoronoiMap
{
    public class VoronoiUtil
    {
        public static List<Vector2Pair> ExtractEdgesFrom(Voronoi voronoi)
        {
            List<Vector2Pair> edges = new List<Vector2Pair>();
            foreach (var item in voronoi.VoronoiDiagram())
            {
                if (item.p0.HasValue && item.p1.HasValue)
                    edges.Add(new Vector2Pair(item.p0.Value, item.p1.Value));
            }

            return edges;
        }
        public static Voronoi GenerateSmoothedVoronoi(int smoothStepCount, MapGenerationData generationData)
        {
            Delaunay.Voronoi voronoi = null;
            for (int i = 0; i < smoothStepCount; i++)
            {
                voronoi = new Delaunay.Voronoi(generationData.Points, generationData.Colors, new Rect(0, 0, generationData.Size.x, generationData.Size.y));
                generationData.Points = voronoi.Relax(generationData.Points);
            }

            return voronoi;
        }
    }


}