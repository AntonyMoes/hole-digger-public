using UnityEngine;

namespace _Game.Scripts.Data.Configs.Level {
    [CreateAssetMenu(menuName = Configs.LevelMenuItem + nameof(LevelConfig), fileName = nameof(LevelConfig))]
    public class LevelConfig : Config {
        [SerializeField] private OreGenerationConfig _oreGenerationConfig;
        public OreGenerationConfig OreGenerationConfig => _oreGenerationConfig;

        [SerializeField] private bool _specialFrontRow;
        public bool SpecialFrontRow => _specialFrontRow;

        [SerializeField] private GameObject _cellPrefab;
        public GameObject CellPrefab => _cellPrefab;

        [SerializeField] private GameObject _frontRowCellPrefab;
        public GameObject FrontRowCellPrefab => _frontRowCellPrefab;

        [SerializeField] private ToolConfig[] _tools;
        public ToolConfig[] Tools => _tools;

        [SerializeField] private int _defaultStrikesToBreak;
        public int DefaultStrikesToBreak => _defaultStrikesToBreak;
    }
}