using System;
using _Game.Scripts.UI.States.Impacts.Base;
using UnityEngine;

namespace _Game.Scripts.UI.States.Impacts {
    public interface IRectTransformImpact : IBaseImpact<RectTransform> { }

    [Serializable]
    public class RectTransformWrapper : ComponentTypeWrapper<RectTransform, IRectTransformImpact> { }

    [Serializable]
    public class RectTransformImpactSetSizeDelta : IRectTransformImpact {
        public Vector2 SizeDelta;

        public void Apply(RectTransform target) {
            target.sizeDelta = SizeDelta;
        }

        public void FillDefaultValues(RectTransform target) {
            SizeDelta = target.sizeDelta.Copy();
        }

        public override string ToString() {
            return "size delta " + SizeDelta;
        }
    }

    [Serializable]
    public class RectTransformImpactSetAnchoredPosition : IRectTransformImpact {
        public Vector2 AnchoredPosition;

        public void Apply(RectTransform target) {
            target.anchoredPosition = AnchoredPosition;
        }

        public void FillDefaultValues(RectTransform target) {
            AnchoredPosition = target.anchoredPosition.Copy();
        }

        public override string ToString() {
            return "anchored pos " + AnchoredPosition;
        }
    }

    [Serializable]
    public class RectTransformImpactSetRotation : IRectTransformImpact {
        public Vector3 Rotation;

        public void Apply(RectTransform target) {
            target.rotation = Quaternion.Euler(Rotation);
        }

        public void FillDefaultValues(RectTransform target) {
            Rotation = target.rotation.eulerAngles.Copy();
        }

        public override string ToString() {
            return "rotation " + Rotation;
        }
    }

    [Serializable]
    public class RectTransformImpactSetScale : IRectTransformImpact {
        public Vector3 Scale;

        public void Apply(RectTransform target) {
            target.localScale = Scale;
        }

        public void FillDefaultValues(RectTransform target) {
            Scale = target.localScale.Copy();
        }

        public override string ToString() {
            return "scale " + Scale;
        }
    }

    [Serializable]
    public class RectTransformImpactSetWidth : IRectTransformImpact {
        public float Width;

        public void Apply(RectTransform target) {
            var sizeDeltaX = target.sizeDelta;
            sizeDeltaX.x = Width;
            target.sizeDelta = sizeDeltaX;
        }

        public void FillDefaultValues(RectTransform target) {
            Width = target.sizeDelta.x;
        }

        public override string ToString() {
            return "width " + Width;
        }
    }

    [Serializable]
    public class RectTransformImpactSetHeight : IRectTransformImpact {
        public float Height;

        public void Apply(RectTransform target) {
            var sizeDeltaX = target.sizeDelta;
            sizeDeltaX.y = Height;
            target.sizeDelta = sizeDeltaX;
        }

        public void FillDefaultValues(RectTransform target) {
            Height = target.sizeDelta.y;
        }

        public override string ToString() {
            return "height " + Height;
        }
    }

    [Serializable]
    public class RectTransformImpactSetTop : IRectTransformImpact {
        public float Top;

        public void Apply(RectTransform target) {
            target.SetTop(Top);
        }

        public void FillDefaultValues(RectTransform target) {
            Top = -target.offsetMax.y;
        }

        public override string ToString() {
            return "top " + Top;
        }
    }

    [Serializable]
    public class RectTransformImpactSetBottom : IRectTransformImpact {
        public float Bottom;

        public void Apply(RectTransform target) {
            target.SetBottom(Bottom);
        }

        public void FillDefaultValues(RectTransform target) {
            Bottom = target.offsetMin.y;
        }

        public override string ToString() {
            return "bottom " + Bottom;
        }
    }

    [Serializable]
    public class RectTransformImpactSetLeft : IRectTransformImpact {
        public float Left;

        public void Apply(RectTransform target) {
            target.SetLeft(Left);
        }

        public void FillDefaultValues(RectTransform target) {
            Left = target.offsetMin.x;
        }

        public override string ToString() {
            return "left " + Left;
        }
    }

    [Serializable]
    public class RectTransformImpactSetRight : IRectTransformImpact {
        public float Right;

        public void Apply(RectTransform target) {
            target.SetRight(Right);
        }

        public void FillDefaultValues(RectTransform target) {
            Right = -target.offsetMax.x;
        }

        public override string ToString() {
            return "right " + Right;
        }
    }

    [Serializable]
    public class RectTransformImpactSetAnchors : IRectTransformImpact {
        public Vector2 AnchorsMin;
        public Vector2 AnchorsMax;

        public void Apply(RectTransform target) {
            target.anchorMin = AnchorsMin;
            target.anchorMax = AnchorsMax;
        }

        public void FillDefaultValues(RectTransform target) {
            AnchorsMin = target.anchorMin.Copy();
            AnchorsMax = target.anchorMax.Copy();
        }

        public override string ToString() {
            return "anchors min " + AnchorsMin + ", max " + AnchorsMax;
        }
    }

    [Serializable]
    public class RectTransformImpactSetPivot : IRectTransformImpact {
        public Vector2 Pivot;

        public void Apply(RectTransform target) {
            target.pivot = Pivot;
        }

        public void FillDefaultValues(RectTransform target) {
            Pivot = target.pivot.Copy();
        }

        public override string ToString() {
            return "pivot " + Pivot;
        }
    }
}