using UnityEngine;

namespace Engine
{
    public class CurrentContextProvider : MonoBehaviour
    {
        public SceneContextDefinition Current { get; private set; }
        internal void Define(SceneContextDefinition definition) => Current = definition;
    }
}
