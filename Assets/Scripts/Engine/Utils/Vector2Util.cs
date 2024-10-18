using System;
using UnityEngine;

namespace Engine
{
    public static class Vector2Util
    {
        public static bool IsInside(this Vector2 pos, Vector2 min, Vector2 max)
        {
            return pos.x > min.x && pos.x < max.x && pos.y > min.y && pos.y < max.y;
        }
        public static Vector2 Swap(this Vector2 vector)
        {
            return new Vector2(vector.y, vector.x);
        }

        public static Quaternion ToRotationFromDirection(this Vector2 direction)
        {
            var rot = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            return Quaternion.Euler(0,0,-rot);
        }
    }
}
