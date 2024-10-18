using EditorAttributes;
using UnityEngine;

namespace Engine
{
    public abstract class ContaineableScriptableObject : ScriptableObject
    {
        public ScriptableObjectContainerBase Container;

        [ReadOnly]
        [SerializeField] private long assetGUID;

        public long AssetGUID => assetGUID;

#if UNITY_EDITOR
        private void OnEnable()
        {
            if (assetGUID == 0)
                assetGUID = GetHashCode();
        }
#endif
    }  
}
