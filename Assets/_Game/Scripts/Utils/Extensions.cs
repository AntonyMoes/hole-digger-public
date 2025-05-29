using GeneralUtils;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.Utils {
    public static class Extensions {
        #region Object

        public static T NullSafe<T>(this T obj) {
            return obj == null || (obj is Object unityObject && unityObject == null) ? default : obj;
        }

        #endregion

        #region Rng

        public static Vector3 NextVector3(this Rng rng, Vector3 from, Vector3 to) {
            return new Vector3(
                rng.NextFloat(from.x, to.x),
                rng.NextFloat(from.y, to.y),
                rng.NextFloat(from.z, to.z)
            );
        }

        #endregion

        #region Ray

        public static float DistanceToPoint(this Ray ray, Vector3 point) {
            return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
        }

        #endregion

        #region LayerMask

        public static bool Contains(this LayerMask mask, int layer) {
            return (mask & (1 << layer)) != 0;
        }

        #endregion

        #region Vector3

        public static Vector3 Inverted(this Vector3 vector) {
            return new Vector3(1 / vector.x, 1 / vector.y, 1 / vector.z);
        }

        #endregion

        #region Transform

        [CanBeNull] public static Transform FindChildByPath(this Transform transform, string childPath) {
            var pathItems = childPath.Split("/");
            var currentItem = transform;
            foreach (var pathItem in pathItems) {
                Transform nextItem = null;
                if (pathItem == "..") {
                    nextItem = currentItem.parent;
                } else {
                    for (var i = 0; i < currentItem.childCount; i++) {
                        var child = currentItem.GetChild(i);
                        if (child.name == pathItem) {
                            nextItem = child;
                            break;
                        }
                    }
                }

                if (nextItem == null) {
                    return null;
                }

                currentItem = nextItem;
            }

            return currentItem;
        }

        public static bool IsAncestorTo(this Transform transform, Transform possibleChild) {
            var parent = possibleChild.parent;

            while (parent != null) {
                if (parent == transform) {
                    return true;
                }

                parent = parent.parent;
            }

            return false;
        }

        #endregion
    }
}