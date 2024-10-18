using System.Collections;
using UnityEngine.SceneManagement;

namespace Engine
{
    public class SceneChangeTaskUnloadScene : ISceneChangeTask
    {
        private int index;

        public SceneChangeTaskUnloadScene(int index)
        {
            this.index = index;
        }

        public IEnumerator Execute(SceneChangeContext _context)
        {
            bool isLoaded = false;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.buildIndex == index)
                    isLoaded = true;
            }

            if (isLoaded)
                yield return SceneManager.UnloadSceneAsync(index);
        }
    }
}
