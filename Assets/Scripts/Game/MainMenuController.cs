using System.Collections;
using System.Collections.Generic;
using EditorAttributes;
using UnityEngine;

namespace Engine
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] SceneContextDefinition playContext;

        [Button] private void LoadPlayContext ()
        {
            CustomSceneManager.Instance.ChangeScene(playContext, new());
        }
    }
}