using System;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.Time;
using GeneralUtils;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.Game.Resource {
    public class ResourceHolder : IResourceHolder, IDisposable {
        private readonly ResourceData _resource;
        private readonly IUpdatedValue<int?> _inventoryCapacity;
        private readonly UpdatedValue<int> _inventorySize;
        private readonly ITimeProvider _timeProvider;
        private readonly Action _save;

        public ResourceConfig Config { get; }

        private readonly UpdatedValue<int> _amount;
        public IUpdatedValue<int> Amount => _amount;

        private readonly UpdatedValue<DateTime?> _nextRefill;
        private readonly UpdatedValue<TimeSpan?> _timeToNextRefill = new UpdatedValue<TimeSpan?>();
        public IUpdatedValue<TimeSpan?> TimeToNextRefill => _timeToNextRefill;
        private readonly UpdatedValue<TimeSpan?> _timeToMax = new UpdatedValue<TimeSpan?>();
        public IUpdatedValue<TimeSpan?> TimeToMax => _timeToMax;

        public int? UpperLimit => Config.ResourceType.UpperLimit;
        public int? InventorySize => Config.ResourceType.InventorySize;
        [CanBeNull] private IResourceType.IAutoRefillSettings AutoRefill => Config.ResourceType.AutoRefill;

        // [CanBeNull] private IResourceLogger _logger;
        private bool _autoUpdating;
        private bool _disposed;

        public ResourceHolder(ResourceConfig config, ResourceData resource, IUpdatedValue<int?> inventoryCapacity,
            UpdatedValue<int> inventorySize, ITimeProvider timeProvider, Action save) {
            Config = config;
            _resource = resource;
            _inventoryCapacity = inventoryCapacity;
            _inventorySize = inventorySize;
            _timeProvider = timeProvider;
            _save = save;

            _amount = new UpdatedValue<int>(_resource.count, LimitAmount);
            _amount.Subscribe(OnAmountUpdate);

            if (InventorySize is { } size) {
                _inventorySize.Value += _resource.count * size;
            }

            DateTime? nextRefill = _resource.nextAutoRefill == DateTime.UnixEpoch
                ? null
                : _resource.nextAutoRefill;
            _nextRefill = new UpdatedValue<DateTime?>(nextRefill);
            _nextRefill.Subscribe(OnNextRefillUpdate, true);
            _timeToNextRefill.Subscribe(OnTimeToNextRefillUpdate, true);

            UpdateNextRefill(_amount.Value);
        }

        private int LimitAmount(int value) {
            var limitCappedValue = UpperLimit is { } upperLimit
                ? Math.Min(upperLimit, value)
                : value;
            var inventoryCappedValue = _inventoryCapacity.Value is { } capacity && InventorySize is { } size &&
                                       _inventorySize.Value > capacity
                ? value - Mathf.CeilToInt((_inventorySize.Value - capacity) / (float) size)
                : value;
            var cappedValue = Math.Min(limitCappedValue, inventoryCappedValue);
            return Math.Max(cappedValue, 0);
        }

        private void OnAmountUpdate(int newAmount) {
            LogChange(_resource.count, newAmount);

            if (InventorySize is { } size) {
                var delta = newAmount - _resource.count;
                _inventorySize.Value += delta * size;
            }

            _resource.count = newAmount;
            _save();
        }

        private void UpdateNextRefill(int amount, int? previousAmount = null) {
            if (amount == previousAmount || UpperLimit is not { } upperLimit) {
                return;
            }

            var initial = previousAmount == null && _nextRefill.Value == null;
            if (amount == upperLimit) {
                _nextRefill.Value = null;
            } else if (previousAmount == upperLimit || initial) {
                _nextRefill.Value = GetNextRefill(null, 1, AutoRefill);
            }
        }

        private void UpdateNextRefillOnTime(DateTime lastRefill, int newAmount, int intervalDelta) {
            _nextRefill.Value = newAmount == UpperLimit
                ? null
                : GetNextRefill(lastRefill, intervalDelta, AutoRefill);
        }

        private void OnNextRefillUpdate(DateTime? nextRefill) {
            _resource.nextAutoRefill = nextRefill ?? DateTime.UnixEpoch;
            _save();

            UpdateTimeToNextRefill();
        }

        private void UpdateTimeToNextRefill() {
            _timeToNextRefill.Value = _nextRefill.Value is { } nextRefill
                ? nextRefill - _timeProvider.CurrentTime
                : null;
        }

        private void OnTimeToNextRefillUpdate(TimeSpan? timeToNextRefill) {
            if (timeToNextRefill is not { } time) {
                _timeToMax.Value = null;
                return;
            }

            if (UpperLimit is not { } upperLimit) {
                _timeToMax.Value = null;
                return;
            }

            var delta = Math.Max(0, upperLimit - _amount.Value);
            _timeToMax.Value = time + (delta - 1) * AutoRefill!.Interval;
        }

        private DateTime? GetNextRefill(DateTime? lastRefill, int intervalDelta,
            [CanBeNull] IResourceType.IAutoRefillSettings autoRefillSettings) {
            if (autoRefillSettings is not { } refillSettings) {
                return null;
            }

            var last = lastRefill ?? _timeProvider.CurrentTime;
            return last + refillSettings.Interval * intervalDelta;
        }

        public bool TryAdd(Resource resource, bool asMuchAsPossible = false) {
            if (!CanAdd(resource, out _, asMuchAsPossible)) {
                return false;
            }

            var previousAmount = _amount.Value;
            _amount.Value += resource.Amount;
            UpdateNextRefill(_amount.Value, previousAmount);
            return true;
        }

        public bool CanAdd(Resource resource, out CantAddReasonType reason, bool asMuchAsPossible = false) {
            if (_disposed) {
                throw new Exception($"Trying to use disposed resource holder {Config.Name}");
            }

            if (resource.Config != Config) {
                reason = CantAddReasonType.WrongResource;
                return false;
            }

            if (UpperLimit is { } limit && Amount.Value + resource.Amount > limit && !asMuchAsPossible) {
                reason = CantAddReasonType.OverLimit;
                return false;
            }

            if (Amount.Value + resource.Amount < 0 && !asMuchAsPossible) {
                reason = CantAddReasonType.NotEnough;
                return false;
            }

            if (!asMuchAsPossible && _inventoryCapacity.Value is { } capacity && InventorySize is { } size &&
                _inventorySize.Value + resource.Amount * size > capacity) {
                reason = CantAddReasonType.InventoryFull;
                return false;
            }

            reason = default;
            return true;
        }

        public void Update() {
            if (_nextRefill.Value is not { } nextRefill) {
                return;
            }

            var now = _timeProvider.CurrentTime;
            var timeDelta = nextRefill - now;

            var refillData = AutoRefill!;
            if (timeDelta < TimeSpan.Zero) {
                var periods = 1 + Convert.ToInt32(Math.Floor(-timeDelta / refillData.Interval));
                _autoUpdating = true;
                _amount.Value += periods * refillData.Amount;
                _autoUpdating = false;
                UpdateNextRefillOnTime(nextRefill, _amount.Value, periods);
            }

            UpdateTimeToNextRefill();
        }

        // public void SetLogger(IResourceLogger logger) {
        //     _logger = logger;
        // }

        private void LogChange(int countBefore, int count) {
            // var type = _autoUpdating
            //     ? ResourceChangeEvent.Type.Timer
            //     : count >= countBefore
            //         ? ResourceChangeEvent.Type.Buy
            //         : ResourceChangeEvent.Type.Spend;
            // _logger?.LogResourceChange(Config, type, countBefore, count);
        }

        public void Dispose() {
            _disposed = true;
        }
    }
}