using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Engine
{
    public class DebugDraw : MonoBehaviour
    {
        internal static void Cross(Vector2 pos, float size = 0.5f, Color color = default, float duration = 0.01f)
        {        
            if (color == default)
                color = Color.white;

            UnityEngine.Debug.DrawLine(pos + new Vector2(size, size), pos + new Vector2(-size, -size), color, duration);
            UnityEngine.Debug.DrawLine(pos + new Vector2(-size, size), pos + new Vector2(size, -size), color, duration);
        }

        internal static void Curve(List<Vector2> points, Color color = default)
        {
            if (color == default)
                color = Color.white;

            for (int i = 1; i < points.Count; i++)
            {
                UnityEngine.Debug.DrawLine(points[i - 1], points[i], color);
                DebugDraw.Cross(points[i], 0.1f, color);
            }
        }
    }
}
