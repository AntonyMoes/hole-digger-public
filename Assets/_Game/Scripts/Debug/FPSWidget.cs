using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.Debug {
    public class FPSWidget : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _fpsHalfSecond;
        [SerializeField] private TextMeshProUGUI _fpsOneSecond;
        [SerializeField] private TextMeshProUGUI _fpsFiveSeconds;
        
        private List<(float interval, TextMeshProUGUI meter)> _fpsMeters;
        private readonly List<float> _frames = new List<float>();

        private float _bufferedTime;
        private float _lastUpdateTime;
        private const float UpdateInterval = 0.1f;

        private void Awake() {
            _fpsMeters = new List<(float interval, TextMeshProUGUI meter)> {
                (0.5f, _fpsHalfSecond),
                (1f, _fpsOneSecond),
                (5f, _fpsFiveSeconds),
            };

            _bufferedTime = _fpsMeters.Select(tuple => tuple.interval).Max();
        }

        public void Update() {
            var time = UnityEngine.Time.time;
            _frames.Add(time);

            if (_lastUpdateTime + UpdateInterval < time) {
                _lastUpdateTime = time;
                foreach (var (interval, meter) in _fpsMeters) {
                    var fps = Mathf.RoundToInt(_frames.Count(stamp => stamp >= time - interval) / interval);
                    meter.text = fps.ToString();
                }
            }

            foreach (var stamp in _frames.ToArray()) {
                if (stamp < time - _bufferedTime) {
                    _frames.Remove(stamp);
                }
            }
        }
    }
}