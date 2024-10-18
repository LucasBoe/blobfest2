using System.Collections;
using UnityEngine;

namespace Engine
{
    public class SceneChangeTaskDestroyGameObject : ISceneChangeTask
    {
        private GameObject go;

        public SceneChangeTaskDestroyGameObject(GameObject _go)
        {
            go = _go;
        }

        public IEnumerator Execute(SceneChangeContext _context)
        {
            GameObject.Destroy(go);
            yield break;
        }
    }
}
