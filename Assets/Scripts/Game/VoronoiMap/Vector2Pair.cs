using UnityEngine;

namespace VoronoiMap
{
    public class Vector2Pair
    {
        public Vector2 A;
        public Vector2 B;
        public Vector2Pair(Vector2 a, Vector2 b)
        {
            A = a;
            B = b;
        }
        public bool Contains(Vector2 check)
        {
            return A == check || B == check;
        }

        public Vector2 Other(Vector2 other)
        {
            if (other == A)
                return B;
            else
                return A;
        }
    }
}