using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Engine
{
    public class BootRedirector : MonoBehaviour
    {
        private void Awake()
        {
            if (SceneManager.sceneCount > 1)
            {
                Destroy(gameObject);
                return;
            }

#if UNITY_EDITOR
            EditorPrefs.SetInt(EditorPref.LAST_OPEN_SCENE, SceneManager.GetActiveScene().buildIndex);
#endif
            SceneManager.LoadScene(0);
        }
    }
}