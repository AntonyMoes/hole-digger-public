using System.Collections.Generic;
using _Game.Scripts.Data.SerializedReference;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Tutorial {
    [CreateAssetMenu(menuName = Configs.TutorialMenuItem + nameof(TutorialConfig), fileName = nameof(TutorialConfig))]
    public class TutorialConfig : Config {
        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private string _uiEntryPoint;
        public string UIEntryPoint => _uiEntryPoint;

        [SerializeReferenceMenu]
        [SerializeReference] private Condition.ConditionValue _condition;

        public Condition.ConditionValue Condition => _condition;

        [SerializeField] private int _priority;
        public int Priority => _priority;

        [SerializeField] private bool _ignoreComplete;
        public bool IgnoreComplete => _ignoreComplete;

        [SerializeField] private bool _instantlyMarkComplete;
        public bool InstantlyMarkComplete => _instantlyMarkComplete;

        [SerializeField] private TutorialStep[] _steps;
        public IReadOnlyList<TutorialStep> Steps => _steps;
        // condition

    }
}