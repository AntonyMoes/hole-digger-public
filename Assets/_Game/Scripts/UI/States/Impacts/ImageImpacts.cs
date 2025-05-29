using System;
using _Game.Scripts.UI.States.Impacts.Base;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI.States.Impacts {
    public interface IImageImpact : IBaseImpact<Image> { }

    [Serializable]
    public class ImageWrapper : ComponentTypeWrapper<Image, IImageImpact> { }

    [Serializable]
    public class ImageImpactSetColor : IImageImpact {
        public Color Color;

        public void Apply(Image target) {
            target.color = Color;
        }

        public void FillDefaultValues(Image target) {
            Color = target.color;
        }

        public override string ToString() {
            return "color #" + ColorUtility.ToHtmlStringRGBA(Color);
        }
    }

    [Serializable]
    public class ImageImpactSetSprite : IImageImpact {
        public Sprite Sprite;

        public void Apply(Image target) {
            target.sprite = Sprite;
        }

        public void FillDefaultValues(Image target) {
            Sprite = target.sprite;
        }

        public override string ToString() {
            return Sprite == null
                ? "empty sprite"
                : "sprite \"" + Sprite.name + "\"";
        }
    }

    [Serializable]
    public class ImageImpactSetMaterial : IImageImpact {
        public Material Material;

        public void Apply(Image target) {
            target.material = Material;
        }

        public void FillDefaultValues(Image target) {
            var material = target.material;
            Material = material == Canvas.GetDefaultCanvasMaterial() ? null : material;
        }

        public override string ToString() {
            return Material == null || Material == Graphic.defaultGraphicMaterial
                ? "default material"
                : "material \"" + Material.name + "\"";
        }
    }

    [Serializable]
    public class ImageImpactSetMaterialFloatValue : IImageImpact {
        public string FloatParam;
        public float FloatValue;

        private Material _materialClone;

        public void Apply(Image target) {
            if (!target.material.HasProperty(FloatParam)) {
                return;
            }

            if (_materialClone == null) {
                if (target.material.name.EndsWith("(Clone)")) {
                    _materialClone = target.material;
                } else {
                    _materialClone = UnityEngine.Object.Instantiate(target.material);
                    target.material = _materialClone;
                }
            }

            _materialClone.SetFloat(FloatParam, FloatValue);
        }

        public void FillDefaultValues(Image target) {
            if (target.material.HasProperty(FloatParam)) {
                FloatValue = target.material.GetFloat(FloatParam);
            }
        }

        public override string ToString() {
            return $"mat. param \"{FloatParam}\" -> \"{FloatValue}\"";
        }
    }
}