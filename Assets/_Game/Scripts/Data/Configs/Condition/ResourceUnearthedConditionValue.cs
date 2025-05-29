using System;
using System.Collections.Generic;
using _Game.Scripts.Condition;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Level;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Condition {
    [Serializable, SerializeReferenceMenuItem(MenuName = "ResourceUnearthed")]
    public class ResourceUnearthedConditionValue : ConditionValue {
        protected override ICondition PerformGetCondition(IContainer container) {
            return container.Create<ResourceUnearthedCondition>();
        }

        private class ResourceUnearthedCondition : Condition {
            [Inject]
            public ResourceUnearthedCondition(ILevelController levelController) {
                levelController.LevelCellsDictionary.Subscribe(OnCellsUpdate, true);
                // Value.Subscribe(val => UnityEngine.Debug.LogError($"UNEARTHED: {val}"), true);
            }

            private void OnCellsUpdate(Dictionary<Vector3Int, ILevelController.LevelCell> cells) {
                var minIndex = int.MaxValue;
                var resources = new List<Vector3Int>();
                foreach (var cell in cells.Values) {
                    minIndex = Math.Min(minIndex, cell.Index.z);
                    if (cell.OreConfigId != null) {
                        resources.Add(cell.Index);
                    }
                }

                foreach (var resource in resources) {
                    var hasCellsInFront = false;
                    for (var z = resource.z - 1; z >= minIndex; z--) {
                        if (cells.ContainsKey(resource.With(z: z))) {
                            hasCellsInFront = true;
                            break;
                        }
                    }

                    if (!hasCellsInFront) {
                        MutableValue.Value = true;
                        return;
                    }
                }

                MutableValue.Value = false;
            }
        }
    }
}