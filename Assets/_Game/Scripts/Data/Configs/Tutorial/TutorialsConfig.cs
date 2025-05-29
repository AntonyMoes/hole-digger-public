using System.Collections.Generic;
using _Game.Scripts.Tutorial;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Tutorial {
    [CreateAssetMenu(menuName = Configs.TutorialMenuItem + nameof(TutorialsConfig), fileName = nameof(TutorialsConfig))]
    public class TutorialsConfig : Config {
        [SerializeField] private TutorialConfig[] _tutorials;
        public IReadOnlyList<TutorialConfig> Tutorials => _tutorials;

        [SerializeField] private TutorialHider _tutorialHiderPrefab;
        public TutorialHider TutorialHiderPrefab => _tutorialHiderPrefab;

        [SerializeField] private TutorialText _tutorialTextPrefab;
        public TutorialText TutorialTextPrefab => _tutorialTextPrefab;
    }
}