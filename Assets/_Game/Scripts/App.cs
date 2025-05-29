using _Game.Scripts.Audio;
using _Game.Scripts.Data.Configs;
using _Game.Scripts.Game;
using _Game.Scripts.Scheduling;
using _Game.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Game.Scripts {
    public class App : MonoBehaviour {
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private EventSystem _eventSystem;
        [SerializeField] private UIControllerView _uiControllerView;
        [SerializeField] private AudioAdapter _audioAdapter;
        [SerializeField] private Scheduler _scheduler;

        private void Start() {
            var loader = new GameLoader(_gameConfig, _scheduler, _eventSystem, _uiControllerView, _audioAdapter);
            loader.Start();
        }
    }
}