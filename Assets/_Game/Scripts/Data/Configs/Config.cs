using _Game.Scripts.Utils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _Game.Scripts.Data.Configs {
    public abstract class Config : ScriptableObject {
        [ReadOnly] [SerializeField] private int _configId;
        public int ConfigId => _configId;

#if UNITY_EDITOR
        public void GenerateId() {
            _configId = IdTool.MakeId(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this)));
        }
#endif
    }
}