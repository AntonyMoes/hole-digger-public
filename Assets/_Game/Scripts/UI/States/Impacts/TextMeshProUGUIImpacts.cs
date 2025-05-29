using System;
using System.ComponentModel;
using _Game.Scripts.UI.States.Impacts.Base;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.UI.States.Impacts {
    public interface ITextMeshProUGUIImpact : IBaseImpact<TextMeshProUGUI> { }

    [Serializable]
    [DisplayName("TextMeshProUGUI")]
    public class TextMeshProUGUIWrapper : ComponentTypeWrapper<TextMeshProUGUI, ITextMeshProUGUIImpact> { }

    [Serializable]
    public class TextMeshProUGUIImpactSetColor : ITextMeshProUGUIImpact {
        public Color Color;

        public void Apply(TextMeshProUGUI target) {
            target.color = Color;
        }

        public void FillDefaultValues(TextMeshProUGUI target) {
            Color = target.color;
        }

        public override string ToString() {
            return "color #" + ColorUtility.ToHtmlStringRGBA(Color);
        }
    }

    [Serializable]
    public class TextMeshProUGUIImpactSetFontSize : ITextMeshProUGUIImpact {
        public float FontSize;

        public void Apply(TextMeshProUGUI target) {
            target.fontSize = FontSize;
        }

        public void FillDefaultValues(TextMeshProUGUI target) {
            FontSize = target.fontSize;
        }

        public override string ToString() {
            return "font size " + FontSize;
        }
    }

    [Serializable]
    public partial class TextMeshProUGUIImpactSetText : ITextMeshProUGUIImpact {
        // TODO: localization
        // [LocalizationString]
        public string TextKey;

        // public string[] TextValues;

        public void Apply(TextMeshProUGUI target) {
            target.SetText(TextKey);
// #if UNITY_EDITOR
// 			target.SetTextEx((Application.isPlaying ? string.Empty : ".") + TextKey.Localize(TextValues));
// #else
// 			target.SetTextEx(TextKey.Localize(TextValues));
// #endif
        }

        public void FillDefaultValues(TextMeshProUGUI target) { }

        public override string ToString() {
            return "key \"" + TextKey + "\"";
            // return "key \"" + TextKey + "\"" + (TextValues.Length > 0 ? "with: " + string.Join("; ", TextValues) : "");
        }
    }

    [Serializable]
    public class TextMeshProUGUIImpactSetFontSizeMin : ITextMeshProUGUIImpact {
        public float FontSizeMin;

        public void Apply(TextMeshProUGUI target) {
            target.fontSizeMin = FontSizeMin;
        }

        public void FillDefaultValues(TextMeshProUGUI target) {
            FontSizeMin = target.fontSizeMin;
        }

        public override string ToString() {
            return "font size min " + FontSizeMin;
        }
    }

    [Serializable]
    public class TextMeshProUGUIImpactSetFontSizeMax : ITextMeshProUGUIImpact {
        public float FontSizeMax;

        public void Apply(TextMeshProUGUI target) {
            target.fontSizeMax = FontSizeMax;
        }

        public void FillDefaultValues(TextMeshProUGUI target) {
            FontSizeMax = target.fontSizeMax;
        }

        public override string ToString() {
            return "font size max " + FontSizeMax;
        }
    }

    [Serializable]
    public class TextMeshProUGUIImpactSetFont : ITextMeshProUGUIImpact {
        public TMP_FontAsset Font;

        public void Apply(TextMeshProUGUI target) {
            target.font = Font;
        }

        public void FillDefaultValues(TextMeshProUGUI target) {
            Font = target.font;
        }

        public override string ToString() {
            return "font " + (Font == null ? " is empty" : "\"" + Font.name + "\"");
        }
    }

    [Serializable]
    public class TextMeshProUGUIImpactSetHorizontalAlignment : ITextMeshProUGUIImpact {
        public HorizontalAlignmentOptions HorizontalAlignment;

        public void Apply(TextMeshProUGUI target) {
            target.horizontalAlignment = HorizontalAlignment;
        }

        public void FillDefaultValues(TextMeshProUGUI target) {
            HorizontalAlignment = target.horizontalAlignment;
        }

        public override string ToString() {
            return "h.alignment " + HorizontalAlignment;
        }
    }

    [Serializable]
    public class TextMeshProUGUIImpactSetVerticalAlignment : ITextMeshProUGUIImpact {
        public VerticalAlignmentOptions VerticalAlignment;

        public void Apply(TextMeshProUGUI target) {
            target.verticalAlignment = VerticalAlignment;
        }

        public void FillDefaultValues(TextMeshProUGUI target) {
            VerticalAlignment = target.verticalAlignment;
        }

        public override string ToString() {
            return "v.alignment " + VerticalAlignment;
        }
    }
}