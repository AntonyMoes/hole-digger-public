using _Game.Scripts.Data;
using UnityEditor;

namespace _Game.Scripts.Editor {
    public static class DataStorageReset {
        [MenuItem(EditorMenu.MenuItem + "Reset data")]
        public static void ResetData() {
            DataStorage.EditorReset();
        }
    }
}