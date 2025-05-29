using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI.States {
    public static class StateUtils {
        public static Vector2 Copy(this Vector2 orig) {
            return new Vector2(orig.x, orig.y);
        }

        public static Vector3 Copy(this Vector3 orig) {
            return new Vector3(orig.x, orig.y, orig.z);
        }

        public static void SetLeft(this RectTransform rt, float left) {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static void SetRight(this RectTransform rt, float right) {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        public static void SetTop(this RectTransform rt, float top) {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static void SetBottom(this RectTransform rt, float bottom) {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }

        public static string GetScenePath(this GameObject obj) {
            var path = "/" + obj.name;

            while (obj.transform.parent != null) {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }

            return path;
        }

        public static void ForceRebuildLayout(this GameObject gameObject) {
            foreach (var group in gameObject.GetComponentsInChildren<LayoutGroup>()) {
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) group.transform);
            }
        }
    }
}