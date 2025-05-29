using System;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Level;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.Input;
using _Game.Scripts.UI.Level;
using GeneralUtils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Game.Scripts.Data.Configs.Works {
    [CreateAssetMenu(menuName = Configs.WorkMenuItem + nameof(LevelWorkConfig), fileName = nameof(LevelWorkConfig))]
    public class LevelWorkConfig : WorkConfig<LevelWorkParameters, LevelWorkFactory> {
        [SerializeField] private GameObject _levelPrefab;

        protected override LevelWorkParameters Parameters => new LevelWorkParameters { LevelPrefab = _levelPrefab };
    }

    public class LevelWorkFactory : IWorkFactory<LevelWorkParameters> {
        private readonly IContainer _container;
        private readonly IInputController _inputController;
        private readonly ILevelController _levelController;
        private readonly IWorldCameraController _worldCameraController;
        private readonly IResourceController _resourceController;

        [Inject]
        public LevelWorkFactory(IContainer container, IInputController inputController,
            ILevelController levelController, IWorldCameraController worldCameraController,
            IResourceController resourceController) {
            _container = container;
            _inputController = inputController;
            _levelController = levelController;
            _worldCameraController = worldCameraController;
            _resourceController = resourceController;
        }

        public bool Do(LevelWorkParameters parameters) {
            if (!parameters.LevelPrefab.TryGetComponent(out LevelView _)) {
                return false;
            }

            var instance = Object.Instantiate(parameters.LevelPrefab);
            var view = instance.GetComponent<LevelView>();
            var presenter = new LevelPresenter(_container, _inputController, _levelController, _worldCameraController,
                view, parameters.OnCantCollectResource);
            parameters.CloseEvent.SubscribeOnce(Close);
            presenter.Start();
            return true;

            void Close() {
                presenter.Stop();
                Object.Destroy(instance);
            }
        }
    }

    public struct LevelWorkParameters : IWorkParameters {
        public GameObject LevelPrefab { get; set; }
        public IEvent CloseEvent { get; set; }
        public Action<CantAddReason, Vector2> OnCantCollectResource { get; set; }
    }
}