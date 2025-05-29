using System;
using _Game.Scripts.DI;
using _Game.Scripts.Utils;

namespace _Game.Scripts.Vibration {
    public class VibrationController : Singleton<VibrationController> {
        [Inject]
        public VibrationController() { }

        public void Vibrate(VibrationType type) {
            switch (type) {
                case VibrationType.None:
                    break;
                case VibrationType.Default:
                    Taptic.Default();
                    break;
                case VibrationType.Selection:
                    Taptic.Selection();
                    break;
                case VibrationType.Warning:
                    Taptic.Warning();
                    break;
                case VibrationType.Win:
                    Taptic.Success();
                    break;
                case VibrationType.Lose:
                    Taptic.Failure();
                    break;
                case VibrationType.Light:
                    Taptic.Light();
                    break;
                case VibrationType.Medium:
                    Taptic.Medium();
                    break;
                case VibrationType.Heavy:
                    Taptic.Heavy();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}