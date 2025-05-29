using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Editor {
    public static class AssetHelper {
        public static IEnumerable<TAsset> LoadAssets<TAsset>() where TAsset : Object {
            return AssetDatabase.FindAssets($"t: {typeof(TAsset).Name}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<TAsset>);
        }
    }
}