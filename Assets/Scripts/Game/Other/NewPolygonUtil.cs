using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class NewPolygonUtil
{
    public static Vector2[] SubdivideUntilMinPointCount(Vector2[] polygon, int pointsToGenerate)
    {
        var subdivided = polygon;
        while (subdivided.Length < pointsToGenerate * 5f)
        {
            subdivided = NewPolygonUtil.Subdivide(subdivided);
        }
        return subdivided;
    }
    internal static Vector2[] Subdivide(Vector2[] p)
    {
        List<Vector2> subdivided = new List<Vector2>();

        for (int a = 0; a < p.Length; a++)
        {
            int b = (a == 0 ? p.Length : a) - 1;

            subdivided.Add(Vector2.Lerp(p[a], p[b], .75f));
            subdivided.Add(Vector2.Lerp(p[a], p[b], .25f));
        }

        return subdivided.ToArray();
    }
    public static Vector2[] GetRoundedPOIs(int numberOfPoints, Vector2[] poly)
    {
        Vector2[] pois = new Vector2[numberOfPoints];
        int[] adds = new int[numberOfPoints];

        for (int i = 0; i < numberOfPoints; i++)
        {
            pois[i] = Vector2.zero;
            adds[i] = 0;
        }

        for (int i = 0; i < poly.Length; i++)
        {
            float bestPOIIndex = ((float)i / poly.Length) * (numberOfPoints - 1);
            int poiIndex = Mathf.RoundToInt(bestPOIIndex);

            pois[poiIndex] += poly[i];
            adds[poiIndex]++;
        }

        for (int i = 0; i < numberOfPoints; i++)
        {
            pois[i] /= adds[i];
        }

        return pois;
    }
    internal static Vector2[] Retrace(Vector2[] poly)
    {
        List<Vector2> retraced = new List<Vector2>();

        float totalDistance = 0f;
        for (int i = 1; i < poly.Length; i++)
            totalDistance += Vector2.Distance(poly[i - 1], poly[i]);

        float singleStepDistance = totalDistance / poly.Length;

        Queue<Vector2> queue = new Queue<Vector2>(poly);

        retraced.Add(queue.Dequeue());

        while (queue.Count > 0)
        {
            Vector2 target = queue.Dequeue();
            while (Vector2.Distance(retraced.Last(), target) >= singleStepDistance)
                retraced.Add(Vector2.MoveTowards(retraced.Last(), target, singleStepDistance));
        }

        retraced.Add(poly.Last());

        return retraced.ToArray();
    }
    internal static Vector2[] LerpToCenter(Vector2[] poly, Vector2 center, float lerp = .3f)
    {
        Vector2[] lerped = new Vector2[poly.Length];
        for (int i = 0; i < poly.Length; i++)
            lerped[i] = Vector2.Lerp(poly[i], center, lerp);
        return lerped;
    }
    internal static Vector2[] CalculateDynamicPointsInPolygon(Vector2[] polygon, int numberOfPoints)
    {
        var worldPosPoly = Cell.TransformGameToTopPos(polygon);
        var center = CalculateCenterPoint(worldPosPoly);
        var subdivided = SubdivideUntilMinPointCount(worldPosPoly, numberOfPoints);
        var draw = Retrace(subdivided);
        var lerped = LerpToCenter(draw, center);
        var pois = GetRoundedPOIs(numberOfPoints, lerped);
        return Cell.TransformToGamePos(pois);
    }

    internal static Vector2 CalculateCenterPoint(Vector2[] polygon)
    {
           var center = Vector2.zero;

            foreach (var edgePoint in polygon)
            {
                center += edgePoint;
            }

            center /= polygon.Length;

        return center;
    }
}