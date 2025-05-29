using System.Collections.Generic;
using _Game.Scripts.Data.Configs;
using UnityEditor;

namespace _Game.Scripts.Editor {
    public class ConfigPostProcessor : AssetPostprocessor {
        public class NewConfigDetector : AssetModificationProcessor
        {
            public static readonly List<string> NewAssets = new List<string>();

            private static void OnWillCreateAsset(string metaPath)
            {
                var path = metaPath[..^5];
                NewAssets.Add(path);
            }
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            if (NewConfigDetector.NewAssets.Count == 0)
                return;

            foreach (var path in importedAssets) {
                if (!NewConfigDetector.NewAssets.Contains(path)) {
                    continue;
                }

                var config = AssetDatabase.LoadAssetAtPath<Config>(path);
                if (config == null) {
                    continue;
                }

                config.GenerateId();
                EditorUtility.SetDirty(config);
            }

            AssetDatabase.SaveAssets();
            NewConfigDetector.NewAssets.Clear();
        }
    }
}
