using System;
using _Game.Scripts.UI.States.Impacts.Base;
using UnityEngine.UI;

namespace _Game.Scripts.UI.States.Impacts {
    public interface ILayoutElementImpact : IBaseImpact<LayoutElement> { }

    [Serializable]
    public class LayoutElementWrapper : ComponentTypeWrapper<LayoutElement, ILayoutElementImpact> { }

    [Serializable]
    public class LayoutElementImpactSetPreferredWidth : ILayoutElementImpact {
        public float PreferredWidth;

        public void Apply(LayoutElement target) {
            target.preferredWidth = PreferredWidth;
        }

        public void FillDefaultValues(LayoutElement target) {
            PreferredWidth = target.preferredWidth;
        }

        public override string ToString() {
            return "pref. width " + PreferredWidth;
        }
    }

    [Serializable]
    public class LayoutElementImpactSetPreferredHeight : ILayoutElementImpact {
        public float PreferredHeight;

        public void Apply(LayoutElement target) {
            target.preferredHeight = PreferredHeight;
        }

        public void FillDefaultValues(LayoutElement target) {
            PreferredHeight = target.preferredHeight;
        }

        public override string ToString() {
            return "pref. height " + PreferredHeight;
        }
    }

    [Serializable]
    public class LayoutElementImpactSetMinWidth : ILayoutElementImpact {
        public float MinWidth;

        public void Apply(LayoutElement target) {
            target.minWidth = MinWidth;
        }

        public void FillDefaultValues(LayoutElement target) {
            MinWidth = target.minWidth;
        }

        public override string ToString() {
            return "min width " + MinWidth;
        }
    }

    [Serializable]
    public class LayoutElementImpactSetMinHeight : ILayoutElementImpact {
        public float MinHeight;

        public void Apply(LayoutElement target) {
            target.minHeight = MinHeight;
        }

        public void FillDefaultValues(LayoutElement target) {
            MinHeight = target.minHeight;
        }

        public override string ToString() {
            return "min height " + MinHeight;
        }
    }

    [Serializable]
    public class LayoutElementImpactSetIgnoreLayout : ILayoutElementImpact {
        public bool IgnoreLayout;

        public void Apply(LayoutElement target) {
            target.ignoreLayout = IgnoreLayout;
        }

        public void FillDefaultValues(LayoutElement target) {
            IgnoreLayout = target.ignoreLayout;
        }

        public override string ToString() {
            return "ignore layout = " + IgnoreLayout;
        }
    }
}