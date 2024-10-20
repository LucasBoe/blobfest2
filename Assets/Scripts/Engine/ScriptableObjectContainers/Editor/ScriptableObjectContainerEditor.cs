#if UNITY_EDITOR
using System;
using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;

namespace Engine
{
    [CustomEditor(typeof(ScriptableObjectContainerBase), editorForChildClasses: true)]
    public class ScriptableObjectContainerEditor : NaughtyInspector
    {
        ScriptableObjectContainerBase container;
        protected override void OnEnable()
        {
            base.OnEnable();
            container = (ScriptableObjectContainerBase)target;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.BeginHorizontal();
            var allowBaseTypeProp = serializedObject.FindProperty("AllowInstancesForBaseType");
            GUILayout.Label("Allow Creation of Instances For Base Type");
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(allowBaseTypeProp, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            var allowOnlyOneInstanceProp = serializedObject.FindProperty("OnlyAllowOneInstancePerChildType");
            GUILayout.Label("Only Allow One Instance Per Child Type");
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(allowOnlyOneInstanceProp, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            GUILayout.EndHorizontal();

            var type = container.ChildClassesBaseType;

            GUILayout.BeginVertical();

            foreach (var _type in EditorUtil.GetAllTypesThatInheritFrom(type, allowBaseTypeProp.boolValue))
            {
                bool show = !container.OnlyAllowOneInstancePerChildType || !container.ContainsElementOfType(_type);

                if (show && GUILayout.Button($"Add {_type.ToShortname()}"))
                {
                    AddNewInstanceOfType(_type);
                }
            }

            GUILayout.EndVertical();

        }

        private void AddNewInstanceOfType(Type type)
        {
            ContaineableScriptableObject containable = container.CreateNewInstanceOfType(type);
            Selection.activeObject = containable;
        }
    }
}
#endif