using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine
{
    public static class CurveUtil
    {
        public static Vector2 CatmullRom(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            var a = Vector2.Lerp(p0, p1, t);
            var b = Vector2.Lerp(p1, p2, t);
            var c = Vector2.Lerp(p2, p3, t);

            var aa = Vector2.Lerp(a, b, t);
            var bb = Vector2.Lerp(b, c, t);

            return Vector2.Lerp(aa, bb, t);
        }
        public static List<Vector2> SpreadSmooth(List<Vector2> points, int spread)
        {
            List<Vector2> smooth = new List<Vector2>() { points[0] };
            int max = points.Count - spread;
            for (int i = 0; i < max; i++)
            {
                float totalWeigth = 0f;
                Vector2 pos = Vector2.zero;
                for (int s = -spread; s <= spread; s++)
                {
                    float weight = Mathf.Pow(s * (1f / spread), 2f) + 1f;
                    int index = Mathf.Clamp(i + s, 0, max - 1);
                    pos += points[index] * weight;
                    totalWeigth += weight;
                }
                pos /= totalWeigth;
                smooth.Add(pos);
            }

            smooth.Add(points.Last());
            return smooth;
        }

        internal static List<Vector2> Average(Vector2 start, Vector3 end, List<Vector2> raw)
        {
            List<Vector2> result = new List<Vector2>();

            float distanceTreshold = 3f;

            var sb = new StringBuilder();
            var distance = Vector2.Distance(start, end);
            int numberOfAverages = Mathf.Max(1, Mathf.RoundToInt(distance / distanceTreshold));

            for (int i = 1; i <= numberOfAverages; i++)
            {
                int min = Mathf.FloorToInt((((float)i - 1f) / numberOfAverages) * (float)raw.Count);
                int max = Mathf.CeilToInt(((float)i / numberOfAverages) * (float)raw.Count);

                Vector2 avg = Vector2.zero;
                int steps = 0;

                for (int j = min; j < max; j++)
                {
                    avg += raw[j];
                    steps++;
                }

                avg /= steps;
                result.Add(avg);

                if (i == 2)
                {
                    DebugDraw.Cross(Vector2.Lerp(start, end, min / (float)raw.Count));
                    DebugDraw.Cross(Vector2.Lerp(start, end, max / (float)raw.Count), color: Color.red);
                }
            }
            return result;
        }
        internal static List<Vector2> LerpSmooth(List<Vector2> points, bool includeStartEnd = false)
        {
            List<Vector2> result = new List<Vector2>();

            if (includeStartEnd)
                result.Add(points.First());

            for (int i = 1; i < points.Count; i++)
            {
                result.Add(Vector2.Lerp(points[i - 1], points[i], 0.33f));
                result.Add(Vector2.Lerp(points[i - 1], points[i], 0.67f));
            }

            if (includeStartEnd)
                result.Add(points.Last());

            return result;
        }
        internal static List<Vector2> LerpSmooth(List<Vector2> points, int loops, bool includeStartEnd = false)
        {
            List<Vector2> result = new List<Vector2>();

            if (includeStartEnd)
                result.Add(points.First());

            List<Vector2> smooth = new List<Vector2>(points);

            for (int i = 0; i < loops; i++)
                smooth = LerpSmooth(smooth, false);

            result.AddRange(smooth);

            if (includeStartEnd)
                result.Add(points.Last());

            return result;
        }
        public static List<Vector2> LerpBetweenCurves(List<Vector2> curveA, List<Vector2> curveB, float t)
        {
            List<Vector2> resultCurve = new List<Vector2>();

            if (curveA.Count == 0)
                return curveB;

            // Ensure both curves have at least one point
            if (curveA.Count < 1 || curveB.Count < 1)
            {
                Debug.LogError("Both curves must have at least one point!");
                return resultCurve;
            }

            // Calculate the number of points in the result curve (the maximum point count of the two curves)
            int maxPointCount = Mathf.Max(curveA.Count, curveB.Count);

            for (int i = 0; i < maxPointCount; i++)
            {
                // Get the corresponding points on each curve based on the index 'i'
                Vector2 pointA = GetCurvePoint(curveA, i);
                Vector2 pointB = GetCurvePoint(curveB, i);

                // Lerp between the points using the given time parameter 't'
                Vector2 lerpedPoint = Vector2.Lerp(pointA, pointB, t);

                // Add the lerped point to the result curve
                resultCurve.Add(lerpedPoint);
            }

            return resultCurve;
        }

        private static Vector2 GetCurvePoint(List<Vector2> curve, int index)
        {
            if (index >= 0 && index < curve.Count)
                return curve[index];

            return curve.Last();
        }

        internal static List<Vector2> Wrap(Vector2 start, Vector2 end, List<Vector2> raw)
        {
            var wrapped = new List<Vector2>() { start };

            foreach (var item in raw)            
                wrapped.Add(item);
            
            wrapped.Add(end);
            return wrapped;
        }
    }
}
