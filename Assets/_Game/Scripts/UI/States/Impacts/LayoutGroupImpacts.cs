using System;
using _Game.Scripts.UI.States.Impacts.Base;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI.States.Impacts {
    public interface ILayoutGroupImpact : IBaseImpact<HorizontalOrVerticalLayoutGroup> { }

    [Serializable]
    public class LayoutGroupWrapper : ComponentTypeWrapper<HorizontalOrVerticalLayoutGroup, ILayoutGroupImpact> { }

    [Serializable]
    public class LayoutGroupImpactSetChildAlignment : ILayoutGroupImpact {
        public TextAnchor ChildAlignment;

        public void Apply(HorizontalOrVerticalLayoutGroup target) {
            target.childAlignment = ChildAlignment;
        }

        public void FillDefaultValues(HorizontalOrVerticalLayoutGroup target) {
            ChildAlignment = target.childAlignment;
        }

        public override string ToString() {
            return "child alignment " + ChildAlignment;
        }
    }

    [Serializable]
    public class LayoutGroupImpactSetReverseArrangement : ILayoutGroupImpact {
        public bool ReverseArrangement;

        public void Apply(HorizontalOrVerticalLayoutGroup target) {
            target.reverseArrangement = ReverseArrangement;
        }

        public void FillDefaultValues(HorizontalOrVerticalLayoutGroup target) {
            ReverseArrangement = target.reverseArrangement;
        }

        public override string ToString() {
            return "rev.arrangement = " + ReverseArrangement;
        }
    }

    [Serializable]
    public class LayoutGroupImpactSetSpacing : ILayoutGroupImpact {
        public float Spacing;

        public void Apply(HorizontalOrVerticalLayoutGroup target) {
            target.spacing = Spacing;
        }

        public void FillDefaultValues(HorizontalOrVerticalLayoutGroup target) {
            Spacing = target.spacing;
        }

        public override string ToString() {
            return "spacing " + Spacing;
        }
    }

    [Serializable]
    public class LayoutGroupImpactSetPadding : ILayoutGroupImpact {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;

        public void Apply(HorizontalOrVerticalLayoutGroup target) {
            target.padding = new RectOffset(Left, Right, Top, Bottom);
        }

        public void FillDefaultValues(HorizontalOrVerticalLayoutGroup target) {
            Left = target.padding.left;
            Right = target.padding.right;
            Top = target.padding.top;
            Bottom = target.padding.bottom;
        }

        public override string ToString() {
            return $"padding ({Left}, {Right}, {Top}, {Bottom})";
        }
    }

    [Serializable]
    public class LayoutGroupImpactSetPaddingTop : ILayoutGroupImpact {
        public int TopPadding;

        public void Apply(HorizontalOrVerticalLayoutGroup target) {
            target.padding.top = TopPadding;
        }

        public void FillDefaultValues(HorizontalOrVerticalLayoutGroup target) {
            TopPadding = target.padding.top;
        }

        public override string ToString() {
            return "padding top " + TopPadding;
        }
    }

    [Serializable]
    public class LayoutGroupImpactSetPaddingBottom : ILayoutGroupImpact {
        public int BottomPadding;

        public void Apply(HorizontalOrVerticalLayoutGroup target) {
            target.padding.bottom = BottomPadding;
        }

        public void FillDefaultValues(HorizontalOrVerticalLayoutGroup target) {
            BottomPadding = target.padding.bottom;
        }

        public override string ToString() {
            return "padding bottom " + BottomPadding;
        }
    }

    [Serializable]
    public class LayoutGroupImpactSetPaddingLeft : ILayoutGroupImpact {
        public int LeftPadding;

        public void Apply(HorizontalOrVerticalLayoutGroup target) {
            target.padding.left = LeftPadding;
        }

        public void FillDefaultValues(HorizontalOrVerticalLayoutGroup target) {
            LeftPadding = target.padding.left;
        }

        public override string ToString() {
            return "padding left " + LeftPadding;
        }
    }

    [Serializable]
    public class LayoutGroupImpactSetPaddingRight : ILayoutGroupImpact {
        public int RightPadding;

        public void Apply(HorizontalOrVerticalLayoutGroup target) {
            target.padding.right = RightPadding;
        }

        public void FillDefaultValues(HorizontalOrVerticalLayoutGroup target) {
            RightPadding = target.padding.right;
        }

        public override string ToString() {
            return "padding right " + RightPadding;
        }
    }

    [Serializable]
    public class LayoutGroupImpactForceRebuildLayout : ILayoutGroupImpact {
        public void Apply(HorizontalOrVerticalLayoutGroup target) {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) target.transform);
        }

        public void FillDefaultValues(HorizontalOrVerticalLayoutGroup target) { }

        public override string ToString() {
            return "rebuild layout";
        }
    }

    [Serializable]
    public class LayoutGroupImpactChildControlSize : ILayoutGroupImpact {
        public bool Width;
        public bool Height;

        public void Apply(HorizontalOrVerticalLayoutGroup target) {
            target.childControlWidth = Width;
            target.childControlHeight = Height;
        }

        public void FillDefaultValues(HorizontalOrVerticalLayoutGroup target) {
            Width = target.childControlWidth;
            Height = target.childControlHeight;
        }

        public override string ToString() {
            return "control size: " +
                   (Width && Height ? "width + height" : Width ? "width" : Height ? "height" : "none");
        }
    }

    [Serializable]
    public class LayoutGroupImpactUseChildScale : ILayoutGroupImpact {
        public bool Width;
        public bool Height;

        public void Apply(HorizontalOrVerticalLayoutGroup target) {
            target.childScaleWidth = Width;
            target.childScaleHeight = Height;
        }

        public void FillDefaultValues(HorizontalOrVerticalLayoutGroup target) {
            Width = target.childScaleWidth;
            Height = target.childScaleHeight;
        }

        public override string ToString() {
            return "use scale: " +
                   (Width && Height ? "width + height" : Width ? "width" : Height ? "height" : "none");
        }
    }

    [Serializable]
    public class LayoutGroupImpactChildForceExpand : ILayoutGroupImpact {
        public bool Width;
        public bool Height;

        public void Apply(HorizontalOrVerticalLayoutGroup target) {
            target.childForceExpandWidth = Width;
            target.childForceExpandHeight = Height;
        }

        public void FillDefaultValues(HorizontalOrVerticalLayoutGroup target) {
            Width = target.childForceExpandWidth;
            Height = target.childForceExpandHeight;
        }

        public override string ToString() {
            return "force expand: " +
                   (Width && Height ? "width + height" : Width ? "width" : Height ? "height" : "none");
        }
    }
}