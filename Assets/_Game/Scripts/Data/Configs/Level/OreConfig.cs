using _Game.Scripts.Data.Configs.Meta.ResourceValue;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Level {
    [CreateAssetMenu(menuName = Configs.LevelMenuItem + nameof(OreConfig), fileName = nameof(OreConfig))]
    public class OreConfig : Config {
        [SerializeField] private ResourceValueConfig _drop;
        public ResourceValueConfig Drop => _drop;

        [SerializeField] private GameObject _dropPrefab;
        public GameObject DropPrefab => _dropPrefab;

        [SerializeField] private GameObject _cellPrefab;
        public GameObject CellPrefab => _cellPrefab;

        [SerializeField] private int _strikesToBreak;
        public int StrikesToBreak => _strikesToBreak;
        
    }
}