using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Engine
{
    public class Booter : MonoBehaviour
    {
        [SerializeField] SceneContextContainer contexts;
        private void Awake()
        {
            Boot();
        }
        internal void Boot()
        {
            var contextToBoot = FindContextToBoot();
            var changeArgs = new SceneChangeArgs()
            {
                Load = false
            };

            CustomSceneManager.Instance.ChangeScene(contextToBoot, changeArgs);
            Destroy(gameObject);
        }
        private SceneContextDefinition FindContextToBoot()
        {
#if UNITY_EDITOR
            if (EditorPrefs.HasKey(EditorPref.LAST_OPEN_SCENE))
            {
                int associatedScene = EditorPrefs.GetInt(EditorPref.LAST_OPEN_SCENE);
                SceneContextDefinition def = GetContextForSceneByBuildIndex(associatedScene);

                if (def != null)
                    return def;

            }
#endif

            return contexts.All[0];
        }

        private SceneContextDefinition GetContextForSceneByBuildIndex(int buildIndex)
        {
            foreach (var context in contexts.All)
            {
                if (context.Scenes.Select(s => s.BuildIndex).Contains(buildIndex))
                    return context;
            }

            return null;
        }
    }
}