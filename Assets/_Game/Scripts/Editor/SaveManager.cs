using System;
using System.Collections.Generic;
using System.IO;
using _Game.Scripts.Data;
using _Game.Scripts.Editor;
using _Game.Scripts.Scheduling;
using GeneralUtils;
using UnityEditor;
using UnityEngine;

namespace Assets._Game.Scripts.Editor {
    public class SaveManager : EditorWindow {
        private const string OldExtension = ".save";
        private const string Extension = ".json";
        private const string DirName = "LocalSaves";
        private string _saveName = "test";
        private readonly List<LocalSave> _localSaves = new List<LocalSave>();
        private DataStorage _dataStorage;

        [MenuItem(EditorMenu.MenuItem + "Save Manager", priority = 1908)]
        public static void Open() {
            var window = GetWindow<SaveManager>();
            window.titleContent.text = "Save Manager";
            // return window;
        }

        private void OnFocus() {
            LoadSaves();
            _dataStorage = new DataStorage(new FakeTimeEventProvider());
        }

        private void OnGUI() {
            GUILayout.Space(10);

            InLineWithOffsets(() => {
                GUILayout.Label(new GUIContent("Название сейва"), EditorStyles.boldLabel, GUILayout.Width(100));
                _saveName = GUILayout.TextField(_saveName);

                GUILayout.Space(10);

                EditorGUI.BeginDisabledGroup(EditorApplication.isUpdating || EditorApplication.isCompiling ||
                                             _saveName.Equals(""));
                if (GUILayout.Button("Сохранить", GUILayout.Width(120))) Save();
                EditorGUI.EndDisabledGroup();
            });

            GUILayout.Space(15);

            InLineWithOffsets(() =>
                GUILayout.Label(new GUIContent("Локальные сейвы"), EditorStyles.boldLabel, GUILayout.Width(110))
            );


            LocalSave toDelete = null;
            foreach (var localSave in _localSaves) {
                InLineWithOffsets(() => {
                    GUILayout.Box(localSave.Name, GUILayout.Width(200), GUILayout.Height(30));

                    EditorGUI.BeginDisabledGroup(EditorApplication.isUpdating || EditorApplication.isCompiling ||
                                                 EditorApplication.isPlaying);
                    if (GUILayout.Button("Загрузить", GUILayout.Width(120), GUILayout.Height(30))) Load(localSave.Name);
                    EditorGUI.EndDisabledGroup();

                    EditorGUI.BeginDisabledGroup(EditorApplication.isUpdating || EditorApplication.isCompiling);
                    if (GUILayout.Button(localSave.WantToDelete ? "Точно удалить" : "Удалить", GUILayout.Width(120),
                            GUILayout.Height(30)))
                        if (localSave.WantToDelete)
                            toDelete = localSave;
                        else
                            localSave.WantToDelete = true;

                    EditorGUI.EndDisabledGroup();
                });

                GUILayout.Space(3);
            }

            if (toDelete != null) Delete(toDelete);
        }

        private static void InLineWithOffsets(Action code) {
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            code();
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
        }

        private void Save() {
            var filePath = GetFilePath(_saveName);

            var data = _dataStorage.EditorLoadData();
            File.WriteAllText(filePath, data);

            LoadSaves();
        }

        private void Load(string save) {
            var filePath = GetFilePath(save);

            if (!File.Exists(filePath)) {
                Debug.LogError($"Could not find save file {save}! Reloading save list");
                LoadSaves();
                return;
            }

            var serializedData = File.ReadAllText(filePath);

            // try {
            //     if (JsonUtility.FromJson<PlayerData>(serializedData) is null) throw new Exception();
            // } catch (Exception) {
            //     Debug.LogError($"Could not parse save {save}!");
            //     return;
            // }

            _dataStorage.EditorSetData(serializedData);
        }

        private void Delete(LocalSave save) {
            _localSaves.Remove(save);

            File.Delete(GetFilePath(save.Name));
        }

        private static void MigrateOldSaves() {
            var dirPath = InitDirPath();
            foreach (var file in Directory.GetFiles(dirPath))
                if (Path.GetExtension(file) == OldExtension)
                    File.Move(file, Path.ChangeExtension(file, Extension));
        }

        private void LoadSaves() {
            MigrateOldSaves();

            _localSaves.Clear();

            var dirPath = InitDirPath();
            foreach (var file in Directory.GetFiles(dirPath))
                if (Path.GetExtension(file) == Extension)
                    _localSaves.Add(new LocalSave {
                        Name = Path.GetFileNameWithoutExtension(file),
                        WantToDelete = false
                    });
        }

        private static string InitDirPath() {
            var dirPath = Path.GetFullPath(".").Replace("\\", "/") + $"/{DirName}/";
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

            return dirPath;
        }

        private static string GetFilePath(string fileName) {
            return Path.ChangeExtension(Path.Combine(InitDirPath(), fileName), Extension);
        }

        private class LocalSave {
            public string Name;
            public bool WantToDelete;
        }

        private class FakeTimeEventProvider : ITimeEventProvider {
            public IEvent<float> FrameEvent { get; } = new Event<float>();

            public IDisposable SubscribeToPeriodicEvent(Action subscriber, float period) {
                return new FakeToken();
            }

            public IEvent<bool> PauseEvent { get; } = new Event<bool>();
            public IEvent QuitEvent { get; } = new GeneralUtils.Event();

            private class FakeToken : IDisposable {
                public void Dispose() { }
            }
        }
    }
}