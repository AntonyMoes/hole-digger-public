using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data;
using _Game.Scripts.Data.Configs;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Leveling;
using _Game.Scripts.Scheduling;
using _Game.Scripts.Time;
using GeneralUtils;

namespace _Game.Scripts.Game.Resource {
    public class ResourceController : IResourceController {
        private const string DataKey = "Resources";
        private const float UpdatePeriod = 1f;

        private readonly InventoryConfig _inventoryConfig;
        private readonly IDataStorage _dataStorage;
        private readonly ITimeProvider _timeProvider;
        private readonly ListData<ResourceData> _data;

        private readonly Dictionary<ResourceConfig, ResourceHolder> _resources =
            new Dictionary<ResourceConfig, ResourceHolder>();
        private readonly IReadOnlyList<IDisposable> _resourceUpdateTokens;
        private readonly IDisposable _inventoryLevelSubscriptionToken;

        private readonly UpdatedValue<int> _inventorySize = new UpdatedValue<int>();
        public IUpdatedValue<int> InventorySize => _inventorySize;

        private readonly UpdatedValue<int?> _inventoryCapacity = new UpdatedValue<int?>();
        public IUpdatedValue<int?> InventoryCapacity => _inventoryCapacity;

        public IReadOnlyList<ResourceConfig> Resources { get; }

        // [CanBeNull] private IResourceLogger _logger;

        [Inject]
        public ResourceController(GameConfig gameConfig, ILevelingController levelingController,
            InventoryConfig inventoryConfig, IScheduler scheduler, IDataStorage dataStorage,
            ITimeProvider timeProvider) {
            _inventoryConfig = inventoryConfig;
            _dataStorage = dataStorage;
            _timeProvider = timeProvider;
            _data = dataStorage.GetData<ListData<ResourceData>>(DataKey);
            _inventoryLevelSubscriptionToken = levelingController.GetLevelData(inventoryConfig).Level
                .Subscribe(OnInventoryLevelUpdate, true);

            Resources = gameConfig.Resources.ToArray();
            _resourceUpdateTokens = Resources
                .Select(resource => scheduler.SubscribeToPeriodicEvent(GetHolder(resource).Update, UpdatePeriod))
                .ToArray();
        }

        private void OnInventoryLevelUpdate(int level) {
            _inventoryCapacity.Value = _inventoryConfig.SizeLevels[level];
        }

        private ResourceHolder GetHolder(ResourceConfig config) {
            return _resources.GetValue(config,
                () => new ResourceHolder(config, _data.GetItem(config.ConfigId), InventoryCapacity, _inventorySize,
                    _timeProvider, Save));
        }

        public IResourceHolder GetResource(ResourceConfig config) {
            return GetHolder(config);
        }

        public bool TryAdd(IResourceValue resources, bool asMuchAsPossible = false) {
            return TryAdd(resources, out _, asMuchAsPossible);
        }

        public bool TryAdd(IResourceValue value, out CantAddReason reason, bool asMuchAsPossible = false) {
            if (!CanAdd(value, out var resourceHolders, out reason, asMuchAsPossible)) {
                return false;
            }

            for (var i = 0; i < value.Value.Count; i++) {
                resourceHolders[i].TryAdd(value.Value[i], asMuchAsPossible);
            }

            return true;
        }

        public bool CanAdd(IResourceValue value, bool asMuchAsPossible = false) {
            return CanAdd(value, out _, out _, asMuchAsPossible);
        }

        public bool CanAdd(IResourceValue value, out CantAddReason reason, bool asMuchAsPossible = false) {
            return CanAdd(value, out _, out reason, asMuchAsPossible);
        }

        private bool CanAdd(IResourceValue value, out List<ResourceHolder> resources, out CantAddReason reason,
            bool asMuchAsPossible) {
            CantAddReasonType? reasonType = null;
            var faultyResources = new List<ResourceConfig>();
            resources = new List<ResourceHolder>();
            foreach (var resourceValue in value.Value) {
                var resource = GetHolder(resourceValue.Config);
                if (!resource.CanAdd(resourceValue, out var resourceReasonType, asMuchAsPossible)) {
                    if (reasonType is { } rt && rt != resourceReasonType) {
                        reason = new CantAddReason(faultyResources, rt);
                        return false;
                    }

                    reasonType = resourceReasonType;
                    faultyResources.Add(resourceValue.Config);
                }

                resources.Add(resource);
            }

            if (reasonType is { } r) {
                reason = new CantAddReason(faultyResources, r);
                return false;
            }

            reason = null;
            return true;
        }

        private void Save() {
            _dataStorage.SetData(_data, DataKey);
        }

        public void Dispose() {
            _inventoryLevelSubscriptionToken.Dispose();
            foreach (var token in _resourceUpdateTokens) {
                token.Dispose();
            }
        }

        // public void SetLogger(IResourceLogger logger) {
        //     foreach (var resource in _resources.Values) {
        //         resource.SetLogger(logger);
        //     }
        // }
    }
}