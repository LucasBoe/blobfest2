using Eflatun.SceneReference;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Engine
{
    [CreateAssetMenu]
    public class SceneContextContainer : ScriptableObjectContainer<SceneContextDefinition>
    {
        [SerializeField] SceneContextDefinition fallback;

        internal SceneContextDefinition FindContextThatContains(int lastOpenSceneIndex)
        {
            foreach (var defintion in Contained)
            {
                foreach (var scenes in defintion.Scenes)
                {
                    if (scenes.BuildIndex == lastOpenSceneIndex)
                        return defintion;
                }
            }

            return fallback;
        }
#if UNITY_EDITOR
        internal void RegisterRoomScene(SceneReference sceneReference)
        {
            foreach (var defintion in Contained)
            {
                if (defintion.name.Contains("Play"))
                {
                    defintion.Scenes.Add(sceneReference);
                    EditorUtility.SetDirty(defintion);
                }
            }
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
