using System;
using System.Linq;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Level;
using _Game.Scripts.Game.Level.Digging;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Tutorial.PathElementSelectors {
    [Serializable, SerializeReferenceMenuItem(MenuName = "UnearthedResource")]
    public class UnearthedResourceSelector : PathElementSelector {
        public override bool Passes(GameObject element, IContainer container) {
            if (!element.TryGetComponent<CellView>(out var view)) {
                return false;
            }

            var cellDictionary = container.Get<ILevelController>().LevelCellsDictionary.Value;
            var index = view.View.Cell.Index;

            var minZ = cellDictionary.Keys.Min(key => key.z);
            for (var z = index.z - 1; z >= minZ; z--) {
                if (cellDictionary.ContainsKey(index.With(z: z))) {
                    return false;
                }
            }

            return true;
        }
    }
}