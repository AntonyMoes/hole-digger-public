using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.Editor.KeystoreLoader {
    public class KeystoreLoaderWindow : EditorWindow {
        private string _dataFile;

        private const string KeyFileExtension = "data";

        [MenuItem(EditorMenu.MenuItem + "KeystoreLoader")]
        private static void Init()
        {
            var window = GetWindow<KeystoreLoaderWindow>(false, "KeystoreLoader");
            window.Show();
        }

        private void Awake() {
            LoadSettings();
        }

        private void OnGUI() {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Data file", GUILayout.Width(100));
            if (GUILayout.Button("Select", GUILayout.Width(70))) {
                _dataFile = EditorUtility.OpenFilePanel("Select keystore data file", "Assets", KeyFileExtension);
            }
            GUILayout.EndHorizontal();

            GUILayout.Label(_dataFile);

            GUILayout.Space(10f);
            if (GUILayout.Button("Apply Changes")) {
                SaveSettings();
            }
        }

        private void LoadSettings() {
            _dataFile = KeystoreSettingsProvider.LoadSettings().FilePath;
        }

        private void SaveSettings() {
            KeystoreSettingsProvider.SaveSettings(new KeystoreSettingsProvider.KeystoreSettings { FilePath = _dataFile });
            Close();
        }
    }
}