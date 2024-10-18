using System.Linq;
using UnityEngine;

namespace Engine
{
    public static class TransformExtensions
    {
        public static void DestroyAllChildrenImmediate(this Transform transform)
        {
            if (transform.childCount <= 0)
                return;

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
            }

        }

        public static void DestroyAllChildren(this Transform transform, params Transform[] exclude)
        {
            if (transform.childCount <= 0)
                return;

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                if (exclude == null || !exclude.Contains(child))
                    GameObject.Destroy(child.gameObject);
            }

        }
        public static Vector3 DirectionTo(this Transform source, Transform destination)
        {
            return source.position.DirectionTo(destination.position);
        }
        public static float DistanceTo(this Transform source, Transform destination)
        {
            return Vector2.Distance(source.position, destination.position);
        }
    }
}
