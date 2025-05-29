using UnityEditor;

namespace _Game.Scripts.Editor.KeystoreLoader {
    public static class KeystoreSettingsProvider {
        public struct KeystoreSettings {
            public string FilePath;
        }

        private static readonly string DataFileId = PlayerSettings.applicationIdentifier + "keystoreDataFile";

        public static KeystoreSettings LoadSettings() {
            return new KeystoreSettings {
                FilePath = EditorPrefs.GetString(DataFileId)
            };
        }

        public static void SaveSettings(KeystoreSettings settings) {
            EditorPrefs.SetString(DataFileId, settings.FilePath);
        }
    }
}