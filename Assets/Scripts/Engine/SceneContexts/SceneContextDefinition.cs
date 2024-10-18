using System.Collections.Generic;
using System.Linq;
using Eflatun.SceneReference;
using UnityEngine;

namespace Engine
{
    [CreateAssetMenu]
    public class SceneContextDefinition : ContaineableScriptableObject
    {
        [SerializeField] List<SceneContextSceneElement> _scenes;
        [SerializeField] GameObject bootSteps;
        [SerializeField] private bool isPlayingScene;

        public bool IsPlayingScene => isPlayingScene;
        public List<SceneReference> Scenes
        {
            get
            {
                return _scenes.Select(s => s.Scene).ToList();
            }
        }
        public IEnumerable<string> GetScenePaths(string[] extraScriptDefines)
        {
            return Scenes.Select(x => x.Path);
        }

        internal GameObject GetBootStepsPrefab()
        {
            if (bootSteps == null)
                return null;
            return bootSteps;
        }
        public List<SceneReference> GetScenesForBooting()
        {
            return Scenes;
        }
        [System.Serializable]
        public class SceneContextSceneElement
        {
            [SerializeField] public SceneReference Scene;
        }
    }
}
