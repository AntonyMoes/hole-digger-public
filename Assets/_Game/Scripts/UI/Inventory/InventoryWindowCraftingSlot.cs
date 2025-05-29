using System;
using System.Collections.Generic;
using _Game.Scripts.Audio;
using _Game.Scripts.Data.Configs;
using _Game.Scripts.Game.Crafting;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.UI.Components;
using _Game.Scripts.UI.Components.ProgressBar;
using _Game.Scripts.UI.Components.Resource;
using _Game.Scripts.UI.States;
using _Game.Scripts.Utils;
using GeneralUtils;
using TMPro;
using UnityEngine;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.UI.Inventory {
    public class InventoryWindowCraftingSlot : MonoBehaviour {
        [SerializeField] private UIState _empty;
        [SerializeField] private UIState _crafting;
        [SerializeField] private UIState _done;
        [SerializeField] private ActiveElement _collectButton;
        [SerializeField] private SoundConfig _doneSound;

        [SerializeField] private ResourceView _resultView;
        [SerializeField] private ProgressBar _progressBar;
        [SerializeField] private TextMeshProUGUI _progressText;

        private readonly List<IDisposable> _subscriptionTokens = new List<IDisposable>();
        private IReadOnlyCrafter _crafter;

        private readonly Event _setRecipeEvent = new Event();
        public IEvent SetRecipeEvent => _setRecipeEvent;

        private readonly Event _collectResultsEvent = new Event();
        public IEvent CollectResultEvent => _collectResultsEvent;


        public void Init(IReadOnlyCrafter crafter, IResourceController resourceController) {
            Clear();

            _crafter = crafter;
            _subscriptionTokens.Add(resourceController.InventorySize.Subscribe(_ => UpdateCollectButton()));
            _subscriptionTokens.Add(crafter.TimeToCompletion.Subscribe(_ => UpdateCollectButton(), true));
            _subscriptionTokens.Add(crafter.TimeToCompletion.Subscribe(UpdateProgress, true));

            _subscriptionTokens.Add(crafter.State.Subscribe(state => UpdateState(state)));
            UpdateState(crafter.State.Value, true);
        }

        private void UpdateState(CrafterState state, bool initial = false) {
            var uiState = state switch {
                CrafterState.Empty => _empty,
                CrafterState.Crafting => _crafting,
                CrafterState.Done => _done,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };

            uiState.Apply();
            if (state == CrafterState.Empty) {
                return;
            }

            _resultView.Setup(_crafter.Reward().Value[0]);

            if (state == CrafterState.Done && !initial) {
                AudioController.Instance.Play(_doneSound);
            }
        }

        private void UpdateCollectButton() {
            _collectButton.SetActive(_crafter.CanCollectResult());
        }

        private void UpdateProgress(TimeSpan? timeLeft) {
            if (timeLeft is not { } time) {
                return;
            }

            _progressText.SetText(time.FormatTimer());

            var duration = _crafter.Time();
            _progressBar.Progress = 1 - (float) (time / duration);
        }

        private void OnDestroy() {
            Clear();
        }

        private void Clear() {
            foreach (var token in _subscriptionTokens) {
                token.Dispose();
            }

            _subscriptionTokens.Clear();
        }

        public void SetRecipe() {
            _setRecipeEvent.Invoke();
        }

        public void CollectResult() {
            _collectResultsEvent.Invoke();
        }
    }
}