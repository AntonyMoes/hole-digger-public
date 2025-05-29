using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Level.Digging;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.Input;
using _Game.Scripts.UI.Level;
using UnityEngine;

namespace _Game.Scripts.Game.Level {
    public class LevelPresenter {
        private readonly IContainer _container;
        private readonly IInputController _inputController;
        private readonly ILevelController _levelController;
        private readonly IWorldCameraController _worldCameraController;
        private readonly LevelView _view;
        private readonly Action<CantAddReason, Vector2> _onCantCollectResource;

        public LevelPresenter(IContainer container, IInputController inputController, ILevelController levelController,
            IWorldCameraController worldCameraController, LevelView view,
            Action<CantAddReason, Vector2> onCantCollectResource) {
            _container = container;
            _inputController = inputController;
            _levelController = levelController;
            _worldCameraController = worldCameraController;
            _view = view;
            _onCantCollectResource = onCantCollectResource;
        }

        public void Start() {
            _inputController.NonUITapEvent.Subscribe(OnTap);
            _worldCameraController.RegisterCamera(_view.Camera);
            _view.Init(_levelController.Depth.Value, _levelController.LevelCells.Value, _container);
            OnCellUpdate(_view.LevelCells);
        }

        public void Stop() {
            _inputController.NonUITapEvent.Unsubscribe(OnTap);
            _worldCameraController.UnregisterCamera(_view.Camera);
        }

        private void OnCellUpdate(IEnumerable<ILevelController.LevelCell> levelCells) {
            _levelController.LevelCells.Value = levelCells.ToArray();
        }

        private void OnTap(Vector2 screenPosition) {
            if (_view.TryGetTapTarget(screenPosition, _levelController.PrioritizeResources.Value, out var result)) {
                if (result.TryGetComponent<CellView>(out var cellView)) {
                    Dig(cellView);
                } else if (result.TryGetComponent<Drop>(out var drop)) {
                    CollectDrop(drop, screenPosition);
                }
            }
        }

        private void Dig(CellView cellView) {
            var row = _view.DigZone.Cells[cellView.View];
            var firstRow = row.RelativeDepth == 0;

            if (row.RelativeDepth >= _view.MaxDigDepth) {
                return;
            }

            var actuallyDug = false;
            foreach (var dugCell in _view.DiggingController.Dig(cellView, row.Index)) {
                _view.DigZone.Remove(dugCell.View);
                actuallyDug = true;
            }

            if (!actuallyDug) {
                return;
            }

            if (row.Cells.Count == 0) {
                _view.DigZone.GenerateNewRow();
            }

            OnCellUpdate(_view.LevelCells);

            if (row.Cells.Count == 0 && firstRow) {
                _levelController.Depth.Value = _view.DigZone.FirstRow.Index;
                _view.AnimateRowFinished();
            }
        }

        private void CollectDrop(Drop drop, Vector2 screenPosition) {
            if (!_levelController.TryCollectDrop(drop.DropValue, out var reason)) {
                _onCantCollectResource?.Invoke(reason, screenPosition);
                return;
            }

            drop.Collect();
        }
    }
}