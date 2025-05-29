using System;
using _Game.Scripts.UI.States.Impacts.Base;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.UI.States.Impacts {
    public interface IGraphicImpact : IBaseImpact<Graphic> { }

    [Serializable]
    public class GraphicWrapper : ComponentTypeWrapper<Graphic, IGraphicImpact> { }

    [Serializable]
    public class GraphicImpactSetMaterial : IGraphicImpact {
        public Material Material;

        public void Apply(Graphic target) {
            target.material = Material;
        }

        public void FillDefaultValues(Graphic target) {
            var material = target.material;
            Material = material == Canvas.GetDefaultCanvasMaterial() ? null : material;
        }

        public override string ToString() {
            return Material == Graphic.defaultGraphicMaterial || Material == null
                ? "default material"
                : "material \"" + Material.name + "\"";
        }
    }
}