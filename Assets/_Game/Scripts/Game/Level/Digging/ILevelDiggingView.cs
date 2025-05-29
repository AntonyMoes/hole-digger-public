using _Game.Scripts.Data.Configs;
using UnityEngine;

namespace _Game.Scripts.Game.Level.Digging {
    public interface ILevelDiggingView {
        public Transform DropParent { get; }
        public Transform ToolParent { get; }
        public LevelEffectController LevelEffectController { get; }
        public SoundConfig DropPickupSound { get; }
    }
}