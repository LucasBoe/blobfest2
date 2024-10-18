#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Engine
{
    [CustomPropertyDrawer(typeof(EventBase), useForChildren: true)]
    public class EventInspector : PropertyDrawer
    {
        private readonly Color highlightColor = new Color(1f, .5f, 0f);
        private const float highlightTime = 1.5f;
        private const float fadeTime = .75f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            try
            {
                EditorGUI.BeginProperty(position, label, property);

                // Cast the target property to use for the highlighting and later for the invoke.
                EventBase targetEventBase = (EventBase)property.GetValue();       

                // Highlight color if recently invoked
                GUIStyle eventNameStyle = EditorStyles.boldLabel;

                Color baseColor = new Color(0.769f, 0.769f, 0.769f, 1f);
                eventNameStyle.normal.textColor = baseColor;

                if (Application.isPlaying && targetEventBase != null)
                {
                    float timeDifference = Time.time - targetEventBase.lastInvoked;

                    if (targetEventBase.lastInvoked > -1f)
                    {
                        if (timeDifference < highlightTime)
                        {
                            eventNameStyle.normal.textColor = highlightColor;
                        }
                        else if (timeDifference - highlightTime <= fadeTime)
                        {
                            float relativeTime = (timeDifference - highlightTime) / fadeTime;
                            eventNameStyle.normal.textColor = Color.Lerp(highlightColor, baseColor, relativeTime);
                        }
                    }
                }

                // Draw label
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Event: " + label.text), eventNameStyle);

                Rect invokeButtonRect = position;
                invokeButtonRect.position = new Vector2((GUI.skin.label.CalcSize(new GUIContent("Event: " + label.text)).x + 30f), position.y);
                invokeButtonRect.width = 50f;
                invokeButtonRect.height = EditorGUIUtility.singleLineHeight + 1f;


                if (Application.isPlaying)
                {
                    // Draw the Invoke button if it is a simple Event.
                    if (targetEventBase is Event targetEvent)
                    {
                        if (GUI.Button(invokeButtonRect, "Invoke"))
                        {
                            targetEvent.Trigger();
                        }
                    }
                }

                // Get the listeners array properties
                SerializedProperty targetsProperty = property.FindPropertyRelative("TargetObjs");

                // Draw the size property
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(position, $"({TryGetArraySize(targetsProperty)})");

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                if (targetsProperty != null)
                {
                    for (int i = 0; i < targetsProperty.arraySize; i++)
                    {
                        SerializedProperty elementProperty = targetsProperty.GetArrayElementAtIndex(i);

                        if (elementProperty.objectReferenceValue == null)
                        {
                            EditorGUI.LabelField(position, "NULL object");
                        }
                        else
                        {
                            EditorGUI.LabelField(position, elementProperty.objectReferenceValue.name);

                            Rect buttonRect = new Rect(position.x + GUI.skin.label.CalcSize(new GUIContent(elementProperty.objectReferenceValue.name)).x + 10f, position.y, 60f, EditorGUIUtility.singleLineHeight + 2.5f);
                            if (GUI.Button(buttonRect, "Highlight"))
                            {
                                EditorGUIUtility.PingObject(elementProperty.objectReferenceValue);
                            }

                            buttonRect.position += new Vector2(60f + 10f, 0);
                            buttonRect.width = 50f;

                            if (GUI.Button(buttonRect, "Select"))
                            {
                                Component selectedObject = elementProperty.objectReferenceValue as Component;

                                if (selectedObject != null)
                                {
                                    // Set the selected object as the active object and ping it
                                    Selection.activeObject = selectedObject;
                                    EditorGUIUtility.PingObject(selectedObject);

                                    EditorWindow focus = EditorWindow.focusedWindow;
                                    //If either window is our inspector, get its current scroll position
                                    if (focus.ToString().Contains("UnityEditor.InspectorWindow") || (focus = EditorWindow.mouseOverWindow).ToString().Contains("UnityEditor.InspectorWindow"))
                                    {
                                        Type T = Type.GetType("UnityEditor.InspectorWindow,UnityEditor");
                                        if (T != null)
                                        {
                                            ScrollView sv = (ScrollView)T.GetField("m_ScrollView", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(focus);
                                            sv.scrollOffset = Vector2.zero;
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning("Subscriber is not a Component, so it cannot be selected!");
                                }
                            }
                        }

                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
                EditorGUI.EndProperty();

                RepaintInspector(property.serializedObject);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error when drawing Event Inspector for property {property.name}");
                Debug.LogException(e);
            }
        }

        private string TryGetArraySize(SerializedProperty property)
        {
            if (property == null)
                return "x";

            return property.arraySize.ToString();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetListHeight(property.FindPropertyRelative("ListenerObjs")) + GetListHeight(property.FindPropertyRelative("TargetObjs"));
        }

        private float GetListHeight(SerializedProperty listProperty)
        {
            if (listProperty == null)
                return EditorGUIUtility.singleLineHeight;

            int listenersCount = listProperty.arraySize;

            // Calculate the total height of the drawer
            float totalHeight = EditorGUIUtility.singleLineHeight;
            totalHeight += listenersCount * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

            return totalHeight;
        }

        private static void RepaintInspector(SerializedObject BaseObject)
        {
            foreach (var item in ActiveEditorTracker.sharedTracker.activeEditors)
                if (item.serializedObject == BaseObject)
                { item.Repaint(); return; }
        }
    }
}
#endif