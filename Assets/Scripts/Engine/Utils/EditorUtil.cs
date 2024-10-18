using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Assertions;

namespace Engine
{
#if UNITY_EDITOR
    public static class EditorUtil
    {
        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
        public static Texture2D CustomEditorIcon(string name)
        {
            return EditorGUIUtility.Load("Assets/Art/Editor/Icons/" + name + ".png") as Texture2D;
        }
        public static Texture2D MakeTex(Color col)
        {
            Color[] pix = new Color[2 * 2];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(2, 2);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        /// <summary>
        /// Returns the first match of type T within the project files
        /// </summary>
        /// <typeparam name="T">Type to look for</typeparam>
        /// <returns></returns>
        public static T FindProjectAssetOfType<T>()
        {
            string guid = AssetDatabase.FindAssets($"t:{typeof(T).ToString()}")[0];
            return (T)(object)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T));
        }

        public static TextAsset FindProjectScriptAssetForType(Type t)
        {
            string guid = AssetDatabase.FindAssets($"{t.Name} t:TextAsset")[0];
            return AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(guid));
        }

        /// <summary>              
        /// Returns the first match of type T within the project files
        /// </summary>
        /// <typeparam name="T">Type to look for</typeparam>
        /// <returns></returns>
        public static IEnumerable<T> FindProjectAssetsOfType<T>()
        {
            var list = new List<T>();

            foreach (var guid in AssetDatabase.FindAssets($"t:{typeof(T).ToString()}"))            
                list.Add((T)(object)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(T)));
            
            return list;
        }
        public static List<Type> GetAllTypesThatInheritFrom(Type type, bool includeBaseType = false)
        {
            System.Type[] types = System.Reflection.Assembly.GetAssembly(type).GetTypes();
            List<Type> possible = new List<Type>();

            if (includeBaseType)
                possible.Add(type);

            possible.AddRange((from System.Type t in types where t.IsSubclassOf(type) select t).ToList());
            return possible;
        }
        // Gets value from SerializedProperty - even if value is nested
        public static object GetValue(this UnityEditor.SerializedProperty property)
        {
            object obj = property.serializedObject.targetObject;

            FieldInfo field = null;
            string[] array = property.propertyPath.Split('.');
            for (int i = 0; i < array.Length; i++)
            {
                string path = array[i];

                var type = obj.GetType();

                if (type.IsArray && path == "Array")
                {
                    path = array[++i]; //= data[index];
                    int index = int.Parse(path.Substring(5, path.Length - 6));
                    obj = ((object[])obj)[index];
                }
                else
                {
                    field = type.GetField(path, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (field == null)
                    {
                        Debug.LogWarning($"Cannot resolve field: {path} in {property.propertyPath}");
                        return null;
                    }

                    obj = field.GetValue(obj);
                }
            }

            return obj;
        }

        // Sets value from SerializedProperty - even if value is nested
        public static void SetValue(this UnityEditor.SerializedProperty property, object val)
        {
            object obj = property.serializedObject.targetObject;

            List<KeyValuePair<FieldInfo, object>> list = new List<KeyValuePair<FieldInfo, object>>();

            FieldInfo field = null;
            foreach (var path in property.propertyPath.Split('.'))
            {
                var type = obj.GetType();
                field = type.GetField(path);
                list.Add(new KeyValuePair<FieldInfo, object>(field, obj));
                obj = field.GetValue(obj);
            }

            // Now set values of all objects, from child to parent
            for (int i = list.Count - 1; i >= 0; --i)
            {
                list[i].Key.SetValue(list[i].Value, val);
                // New 'val' object will be parent of current 'val' object
                val = list[i].Value;
            }
        }
        public static GUIStyle AlternatingBackground(int rowIndex, GUIStyle previousStyle = null)
        {
            Color[] colors = new Color[] { Color.clear, new Color(0.2f, 0.2f, 0.2f, 1) };
            GUIStyle style = CreateNewStyleFrom(previousStyle);
            style.normal.background = MakeTex(colors[rowIndex % 2]);
            return style;
        }
        public static GUIStyle CreateNewStyleWithBackgroundColor(this GUIStyle previousStyle, Color color)
        {
            GUIStyle style = CreateNewStyleFrom(previousStyle);
            style.normal.background = MakeTex(color);
            return style;
        }
        private static GUIStyle CreateNewStyleFrom(GUIStyle previousStyle)
        {
            return previousStyle == null ? new GUIStyle() : new GUIStyle(previousStyle);
        }

        public static GUIStyle Padding(int padding, GUIStyle previousStyle = null)
        {
            GUIStyle style = CreateNewStyleFrom(previousStyle);
            style.padding = new RectOffset(padding, padding, padding, padding);
            return style;
        }
        public static GUIStyle Padding(int paddingX, int paddingY, GUIStyle previousStyle = null)
        {
            GUIStyle style = CreateNewStyleFrom(previousStyle);
            style.padding = new RectOffset(paddingX, paddingX, paddingY, paddingY);
            return style;
        }
        public static GUIStyle Padding(int paddingLeft, int paddingRight, int paddingTop, int paddingBottom, GUIStyle previousStyle = null)
        {
            GUIStyle style = CreateNewStyleFrom(previousStyle);
            style.padding = new RectOffset(paddingLeft, paddingRight, paddingTop, paddingBottom);
            return style;
        }

    }
#endif // UNITY_EDITOR
}