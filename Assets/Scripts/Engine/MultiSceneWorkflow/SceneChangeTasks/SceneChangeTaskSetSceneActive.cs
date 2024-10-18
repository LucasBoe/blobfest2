using System.Collections;
using UnityEngine.SceneManagement;

namespace Engine
{
    public class SceneChangeTaskSetSceneActive : ISceneChangeTask
    {
        private int index;

        public SceneChangeTaskSetSceneActive(int index)
        {
            this.index = index;
        }

        public IEnumerator Execute(SceneChangeContext _context)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(index));
            yield break;
        }
    }
}
