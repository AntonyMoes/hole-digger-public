using System;
using _Game.Scripts.UI.States.Impacts.Base;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI.States.Impacts {
    public interface IRawImageImpact : IBaseImpact<RawImage> { }

    [Serializable]
    public class RawImageWrapper : ComponentTypeWrapper<RawImage, IRawImageImpact> { }

    [Serializable]
    public class RawImageImpactSetColor : IRawImageImpact {
        public Color Color;

        public void Apply(RawImage target) {
            target.color = Color;
        }

        public void FillDefaultValues(RawImage target) {
            Color = target.color;
        }

        public override string ToString() {
            return "color #" + ColorUtility.ToHtmlStringRGBA(Color);
        }
    }

    [Serializable]
    public class RawImageImpactSetTexture : IRawImageImpact {
        public Texture Texture;

        public void Apply(RawImage target) {
            target.texture = Texture;
        }

        public void FillDefaultValues(RawImage target) {
            Texture = target.texture;
        }

        public override string ToString() {
            return Texture == null
                ? "empty texture"
                : "texture \"" + Texture.name + "\"";
        }
    }

    [Serializable]
    public class RawImageImpactSetMaterialFloatValue : IRawImageImpact {
        public string FloatParam;
        public float FloatValue;

        private Material _materialClone;

        public void Apply(RawImage target) {
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

        public void FillDefaultValues(RawImage target) {
            if (target.material.HasProperty(FloatParam)) {
                FloatValue = target.material.GetFloat(FloatParam);
            }
        }

        public override string ToString() {
            return $"mat. param \"{FloatParam}\" -> \"{FloatValue}\"";
        }
    }
}