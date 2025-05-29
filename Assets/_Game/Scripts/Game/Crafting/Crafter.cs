using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.Time;
using _Game.Scripts.Utils;
using GeneralUtils;

namespace _Game.Scripts.Game.Crafting {
    public class Crafter : ICrafter {
        private readonly CraftingGroupData.CraftingProcess _process;
        private readonly IResourceController _resourceController;
        private readonly ITimeProvider _timeProvider;
        private readonly Action _save;

        private readonly UpdatedValue<CrafterState> _state = new UpdatedValue<CrafterState>();
        public IUpdatedValue<CrafterState> State => _state;

        private readonly UpdatedValue<CraftingConfig> _currentProcess = new UpdatedValue<CraftingConfig>();
        public IUpdatedValue<CraftingConfig> CurrentProcess => _currentProcess;
        public int Amount { get; private set; }

        private readonly UpdatedValue<DateTime?> _completesAt = new UpdatedValue<DateTime?>();
        private readonly UpdatedValue<TimeSpan?> _timeToCompletion = new UpdatedValue<TimeSpan?>();
        public IUpdatedValue<TimeSpan?> TimeToCompletion => _timeToCompletion;

        public Crafter(IEnumerable<CraftingConfig> recipes, CraftingGroupData.CraftingProcess process,
            IResourceController resourceController, ITimeProvider timeProvider, Action save) {
            _process = process;
            _resourceController = resourceController;
            _timeProvider = timeProvider;
            _save = save;

            _currentProcess.Value = process.configId == 0
                ? null
                : recipes.First(config => config.ConfigId == process.configId);
            Amount = process.count;
            _completesAt.Value = process.completesAt == DateTime.UnixEpoch ? null : process.completesAt;
            _completesAt.Subscribe(_ => UpdateTimeToCompletion());
            _state.Value = _currentProcess.Value == null
                ? CrafterState.Empty
                : _completesAt.Value == null
                    ? CrafterState.Done
                    : CrafterState.Crafting;

            Update();
        }

        public bool CanStartCrafting(CraftingConfig config, int amount, out CantCraftReason reason) {
            if (_currentProcess.Value != null) {
                reason = new CantCraftReason(CantCraftReasonType.Busy);
                return false;
            }

            if (!_resourceController.CanAdd(config.Price(amount), out var cantAddReason)) {
                reason = new CantCraftReason(CantCraftReasonType.CantAdd, cantAddReason);
                return false;
            }

            reason = null;
            return true;
        }

        public bool TryStartCrafting(CraftingConfig config, int amount, out CantCraftReason reason) {
            if (!CanStartCrafting(config, amount, out reason) || !_resourceController.TryAdd(config.Price(amount))) {
                return false;
            }

            _currentProcess.Value = config;
            Amount = amount;
            _completesAt.Value = _timeProvider.CurrentTime + this.Time();

            _state.Value = CrafterState.Crafting;
            Save();

            return true;
        }

        public bool CanCollectResult() {
            if (_currentProcess.Value == null || _completesAt.Value != null) {
                return false;
            }

            if (!_resourceController.CanAdd(this.Reward(), out _)) {
                return false;
            }

            return true;
        }

        public bool TryCollectResult() {
            if (!CanCollectResult() || !_resourceController.TryAdd(this.Reward(), out _)) {
                return false;
            }

            _currentProcess.Value = null;
            Amount = 0;
            
            _state.Value = CrafterState.Empty;
            Save();

            return true;
        }

        private void Save() {
            _process.configId = _currentProcess.Value.NullSafe()?.ConfigId ?? 0;
            _process.count = Amount;
            _process.completesAt = _completesAt.Value ?? DateTime.UnixEpoch;
            _save();
        }

        public void Update() {
            if (_completesAt.Value is not { } completesAt) {
                return;
            }

            var now = _timeProvider.CurrentTime;
            var timeDelta = completesAt - now;
            if (timeDelta <= TimeSpan.Zero) {
                _completesAt.Value = null;
                _state.Value = CrafterState.Done;
                return;
            }

            UpdateTimeToCompletion();
        }

        private void UpdateTimeToCompletion() {
            _timeToCompletion.Value = _completesAt.Value is { } completesAt
                ? completesAt - _timeProvider.CurrentTime
                : null;
        }
    }
}