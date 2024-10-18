using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Engine
{
    public static class Util
    {
        public static Material CreateMaterialInstance(this SpriteRenderer spriteRenderer)
        {
            var mat = new Material(spriteRenderer.material);
            spriteRenderer.material = mat;
            return mat;
        }
        public static T[] FindAllThatImplementToArray<T>()
        {
            return UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<T>().ToArray();
        }

        public static IEnumerable<T> FindAllThatImplement<T>()
        {
            return SingletonManager.Instance.FindRawSingletonsThatImplement<T>().Concat(
                UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<T>());
        }
        public static T AddCloneComponentOf<T>(this GameObject destination, T original) where T : Component
        {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);
            System.Reflection.FieldInfo[] fields = type.GetFields();
            System.Reflection.PropertyInfo[] properties = type.GetProperties();
            Debug.Log($"copy properties ({properties.Length}) from type {type.Name}");
            foreach (System.Reflection.PropertyInfo property in properties)
            {
                try
                {
                    var value = property.GetValue(original);
                    property.SetValue(copy, value);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
            return copy as T;
        }
        public static IEnumerable<T> InvokeFunctionsOnChangingElements<T>(this IEnumerable<T> before, IEnumerable<T> after, Action<T> onEnter = null, Action<T> onExit = null)
        {
            List<T> all = new List<T>(before);

            foreach (var item in after)
            {
                if (!before.Contains(item))
                    all.Add(item);
            }

            foreach (var item in all)
            {
                if (before.Contains(item) && !after.Contains(item))
                {
                    onExit?.Invoke(item);

                }
                else if (!before.Contains(item) && after.Contains(item))
                {
                    onEnter?.Invoke(item);
                }
            }
            return after;
        }
        public static T GetRandom<T>(this IEnumerable<T> list)
        {
            int max = list.Count();

            if (max == 0)
                return default;

            return list.ToArray()[UnityEngine.Random.Range(0, max)];
        }
        public static T[] AsArray<T>(this T element)
        {
            return new T[] { element };
        }
        public static string ToMMSS(this float timeInSeconds)
        {
            return TimeSpan.FromSeconds(timeInSeconds).ToString(@"mm\:ss");
        }
        public static bool IsOfType<T>(this object obj) => obj.GetType() == typeof(T);
    }
}