using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Engine
{
    static class Extensions
    {
        public static float Interpolate(float before, float target, float duration)
        {
            return Mathf.Lerp(before, target, Time.deltaTime / duration);
        }
        public static Vector2 Interpolate(Vector2 before, Vector2 target, float duration)
        {
            return new Vector2(
                Mathf.Lerp(before.x, target.x, Time.deltaTime / duration),
                Mathf.Lerp(before.y, target.y, Time.deltaTime / duration)
                );
        }

        //Find all instances of an Interface in the scene
        public static List<T> Find<T>()
        {
            List<T> interfaces = new List<T>();
            GameObject[] rootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var rootGameObject in rootGameObjects)
            {
                T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
                foreach (var childInterface in childrenInterfaces)
                {
                    interfaces.Add(childInterface);
                }
            }

            return interfaces;
        }

        //Get next in Enum
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }

        public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;

            randDirection += origin;

            NavMeshHit navHit;

            NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

            return navHit.position;
        }

        public static float Remap(float input, float oldLow, float oldHigh, float newLow, float newHigh)
        {
            float t = Mathf.InverseLerp(oldLow, oldHigh, input);
            return Mathf.Lerp(newLow, newHigh, t);
        }
    }

    public static class Vector3Extensions
    {
        public static Vector3 With(this Vector3 original, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? original.x, y ?? original.y, z ?? original.z);
        }
        
        public static Vector2 With(this Vector2 original, float? x = null, float? y = null)
        {
            return new Vector3(x ?? original.x, y ?? original.y);
        }

        public static Vector3 Flat(this Vector3 original)
        {
            return new Vector3(original.x, 0, original.z);
        }

        public static Vector3 DirectionTo(this Vector3 source, Vector3 destination)
        {
            return Vector3.Normalize(destination - source);
        }

        public static Vector3 Abs(this Vector3 original)
        {
            return new Vector3(Mathf.Abs(original.x), Mathf.Abs(original.y), Mathf.Abs(original.z));
        }

        public static Vector3 Random(Vector3 min, Vector3 max)
        {
            return new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y),
                UnityEngine.Random.Range(min.z, max.z));
        }
    }

    public static class ColorExtensions
    {
        public static Color With(this Color original, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            return new Color(r ?? original.r, g ?? original.g, b ?? original.b, a ?? original.a);
        }

        public static Color Where(this Color original, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            return new Color(r == null ? original.r : r.Value, g == null ? original.g : g.Value, b == null ? original.b : b.Value, a == null ? original.a : a.Value);
        }
    }

    public static class AnimatorExtensions
    {
        public static bool FinishedAnimation(this Animator animator, int layer, string animationStateName)
        {
            return !animator.IsInTransition(0) && !animator.GetCurrentAnimatorStateInfo(0).IsName(animationStateName);
        }
    }


    public static class RaycastExtensions
    {
        public static RaycastHit RayCastFromCamera(int layerMask)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask);
            UnityEngine.Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

            return hit;
        }
    }

    public class MathParabola
    {
        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector3.Lerp(start, end, t);

            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }

        public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

            var mid = Vector2.Lerp(start, end, t);

            return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
        }
    }

    public class CameraExtensions
    {
        public static bool IsSceneViewCameraInRange(Vector3 position, float distance)
        {
            Vector3 cameraPos = Camera.current.WorldToScreenPoint(position);
            return ((cameraPos.x >= 0) &&
                    (cameraPos.x <= Camera.current.pixelWidth) &&
                    (cameraPos.y >= 0) &&
                    (cameraPos.y <= Camera.current.pixelHeight) &&
                    (cameraPos.z > 0) &&
                    (cameraPos.z < distance));
        }
    }

    public static class Texture2DExtensions
    {
        public static void DrawCircleOnTexture(Texture2D tex, Color color, int x, int y, int radius = 1)
        {
            float rSquared = radius * radius;

            for (int u = x - radius; u < x + radius + 1; u++)
            {
                for (int v = y - radius; v < y + radius + 1; v++)
                {
                    if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                    {
                        if(u > 0 && u < tex.width && v > 0 && v < tex.height)
                        {
                            tex.SetPixel(u, v, color);
                        }
                    }
                }
            }

            tex.Apply();
        }
        
        public static Texture2D ChangeFormat(this Texture2D oldTexture, TextureFormat newFormat)
        {
            //Create new empty Texture
            Texture2D newTex = new Texture2D(oldTexture.width, oldTexture.height, newFormat, false);
            //Copy old texture pixels into new one
            newTex.SetPixels(oldTexture.GetPixels());
            //Apply
            newTex.Apply();

            return newTex;
        }
    }

    public static class EditorWindowExtensions
    {
        public static GUIStyle CreateOrAdjustStyleWithColor(Color color, GUIStyle guiStyle = null)
        {
            GUIStyle resultStyle = new();
            if (guiStyle != null) resultStyle = guiStyle;

            resultStyle.normal.textColor = color;
            resultStyle.onNormal.textColor = color;
            resultStyle.hover.textColor = color;
            resultStyle.onHover.textColor = color;
            resultStyle.active.textColor = color;
            resultStyle.onActive.textColor = color;
            resultStyle.focused.textColor = color;
            resultStyle.onFocused.textColor = color;

            return resultStyle;
        }
    }
}