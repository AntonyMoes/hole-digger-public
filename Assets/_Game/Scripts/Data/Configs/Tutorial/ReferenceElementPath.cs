using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using _Game.Scripts.Data.Configs.Tutorial.PathElementSelectors;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.DI;
using _Game.Scripts.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Tutorial {
    [Serializable]
    public class ReferenceElementPath {
        [SerializeField] private string _path;
        [SerializeReferenceMenu]
        [SerializeReference] private PathElementSelector[] _selectors;
        [SerializeField] private bool _follow;
        public bool Follow => _follow;

        private static readonly Regex ElementWithSelectorRegex = new Regex("\\[\\[\\{?(.*)\\}?\\]\\]");
        private static readonly Regex ElementWithBranchingRegex = new Regex("\\{(.*)\\}");

        [CanBeNull]
        public Transform Find([CanBeNull] Transform root, IContainer container) {
            var pathItems = _path.Split("/");
            var currentItem = root;
            var startChildIndex = 0;
            var branchStack = new Stack<(Transform, int, int, int)>();
            var selectorIndex = 0;
            for (var pathIndex = 0; pathIndex < pathItems.Length; pathIndex++) {
                var pathItem = pathItems[pathIndex];
                Transform nextItem = null;
                if (pathItem == "" && pathIndex == 0) {
                    // root can be null
                    pathIndex += 1;
                    pathItem = pathItems[pathIndex];
                    nextItem = GameObject.Find("/" + pathItem).NullSafe()?.transform;
                } else if (pathItem == "..") {
                    nextItem = currentItem.parent;
                } else {
                    var selectorIndexBefore = selectorIndex;
                    var selectorMatch = ElementWithSelectorRegex.Match(pathItem);
                    var branchingMatch = ElementWithBranchingRegex.Match(pathItem);

                    Func<GameObject, bool> elementChecker;
                    string elementName;
                    if (selectorMatch.Success) {
                        var selectorIdx = selectorIndex;
                        elementChecker = element => _selectors[selectorIdx].Passes(element, container);
                        selectorIndex += 1;

                        elementName = selectorMatch.Groups[1].Value;
                    } else {
                        elementChecker = _ => true;
                        elementName = branchingMatch.Success
                            ? branchingMatch.Groups[1].Value
                            : pathItem;
                    }

                    var elementNames = elementName.Split("|").ToHashSet();

                    var startIndex = startChildIndex;
                    startChildIndex = 0;
                    for (var i = startIndex; i < currentItem.childCount; i++) {
                        var child = currentItem.GetChild(i);
                        if (elementNames.Contains(child.name) && elementChecker(child.gameObject)) {
                            nextItem = child;

                            if (branchingMatch.Success) {
                                branchStack.Push((currentItem, pathIndex - 1, nextItem.GetSiblingIndex() + 1, selectorIndexBefore));
                            }

                            break;
                        }
                    }
                }

                if (nextItem == null) {
                    if (branchStack.Count == 0) {
                        return null;
                    }

                    (nextItem, pathIndex, startChildIndex, selectorIndex) = branchStack.Pop();
                }

                currentItem = nextItem;
            }

            return currentItem;
        }

        public override string ToString() {
            return $"{nameof(ReferenceElementPath)}: ({_path})";
        }
    }
}