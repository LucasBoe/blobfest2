using System.Collections;
using UnityEngine.SceneManagement;

namespace Engine
{
    public class SceneChangeTaskLoadScene : ISceneChangeTask
    {
        private int index;

        public SceneChangeTaskLoadScene(int index)
        {
            this.index = index;
        }

        public IEnumerator Execute(SceneChangeContext _context)
        {
            yield return SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        }
    }
}
