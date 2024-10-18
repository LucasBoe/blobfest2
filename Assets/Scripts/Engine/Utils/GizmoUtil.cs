using UnityEngine;

namespace Engine
{
    public static class GizmoUtil
    {
        public static void DrawWhireCapsule2D(Vector2 pos, Vector2 size)
        {
            var direction = size.x > size.y ? CapsuleDirection2D.Horizontal : CapsuleDirection2D.Vertical;
            float step = 0.2f;
            float r = Mathf.Min(size.x, size.y) / 2;
            float d = Mathf.Max(size.x, size.y) / 2 - r;
            Vector3 lp = Vector3.positiveInfinity;
            for (float a = -Mathf.PI; a <= Mathf.PI + step; a += step)
            {
                float x = Mathf.Cos(a);
                float y = Mathf.Sin(a);
                Vector3 np = new Vector3(x, y) * r;
                np += (direction == CapsuleDirection2D.Vertical ? Vector3.up : Vector3.zero) * d * Mathf.Sign(y);
                np += (direction == CapsuleDirection2D.Horizontal ? Vector3.right : Vector3.zero) * d * Mathf.Sign(x);
                np = (Vector3)pos + np;
                if (lp != Vector3.positiveInfinity) Gizmos.DrawLine(lp, np);
                lp = np;
            }
        }
        public static void DrawCross(Vector2 pos, float size = 0.5f)
        {
            Gizmos.DrawLine(pos + new Vector2(size, size), pos + new Vector2(-size, -size));
            Gizmos.DrawLine(pos + new Vector2(-size, size), pos + new Vector2(size, -size));
        }
    }
}
