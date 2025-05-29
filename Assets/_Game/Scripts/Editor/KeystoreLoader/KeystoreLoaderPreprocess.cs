#if UNITY_ANDROID
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace _Game.Scripts.Editor.KeystoreLoader {
    public class KeystoreLoaderPreprocess : IPreprocessBuildWithReport {
        public int callbackOrder => 2;

        public void OnPreprocessBuild(BuildReport report) {
            var dataPath = KeystoreSettingsProvider.LoadSettings().FilePath;
            LoadKeystoreData(dataPath);
            PrintKeystore();
        }

        private static void LoadKeystoreData(string dataPath) {
            if (!File.Exists(dataPath)) {
                UnityEngine.Debug.Log("Keystore data file not found");
                return;
            }

            var data = JsonUtility.FromJson<KeystoreData>(File.ReadAllText(dataPath));

            PlayerSettings.Android.keystoreName = data.keystoreName;
            PlayerSettings.keystorePass = data.keystorePass;
            PlayerSettings.Android.keyaliasName = data.keyaliasName;
            PlayerSettings.keyaliasPass = data.keyaliasPass;
        }

        private static void PrintKeystore() {
            Console.WriteLine($"PlayerSettings.Android.keystoreName = {PlayerSettings.Android.keystoreName}");
            Console.WriteLine($"PlayerSettings.keystorePass = {PlayerSettings.keystorePass}");
            Console.WriteLine($"PlayerSettings.Android.keyaliasName = {PlayerSettings.Android.keyaliasName}");
            Console.WriteLine($"PlayerSettings.keyaliasPass = {PlayerSettings.keyaliasPass}");
            Console.WriteLine($"PlayerSettings.Android.useCustomKeystore = {PlayerSettings.Android.useCustomKeystore}");
        }

        [Serializable]
        private class KeystoreData {
            public string keystoreName;
            public string keystorePass;
            public string keyaliasName;
            public string keyaliasPass;
        }
    }
}
#endif