#if UNITY_EDITOR
using System.Text.RegularExpressions;
using EditorAttributes.Editor;
using UnityEditor;
using UnityEngine;

namespace Engine
{
    [CustomEditor(typeof(ContaineableScriptableObject), editorForChildClasses: true)]
    public class ContaineableScriptableObjectEditor : EditorExtension
    {
        private bool showRename = false;
        private string newName;
        private ContaineableScriptableObject containable;

        protected override void OnEnable()
        {
            base.OnEnable();
            containable = target as ContaineableScriptableObject;
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ScriptableObjectContainerBase container = serializedObject.FindProperty("Container").objectReferenceValue as ScriptableObjectContainerBase;
            if (container != null && GUILayout.Button("Destroy"))
            {
                if (EditorUtility.DisplayDialog("Deletion", "Are you sure you want to delete?", "Delete", "Cancel"))
                {
                    container.Destroy(target as ContaineableScriptableObject);
                    AssetDatabase.SaveAssets();
                    Selection.activeObject = container;
                }
            }

            if (!showRename)
            {
                if (container != null && GUILayout.Button("Rename"))
                {
                    showRename = true;
                    newName = containable.name;
                }
            }
            
            if (showRename)
            {
                EditorGUILayout.Space(5f);
                newName = EditorGUILayout.TextField("New Name:", newName);
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Apply"))
                {
                    containable.name = $"{newName}";
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorApplication.RepaintProjectWindow();
                    showRename = false;
                }

                if (GUILayout.Button("Cancel"))
                {
                    showRename = false;
                }
                
                GUILayout.EndHorizontal();
            }
        }
    }
}
#endif