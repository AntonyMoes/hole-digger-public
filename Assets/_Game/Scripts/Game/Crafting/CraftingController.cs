using System;
using System.Collections.Generic;
using _Game.Scripts.Data;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.Scheduling;
using _Game.Scripts.Time;
using GeneralUtils;

namespace _Game.Scripts.Game.Crafting {
    public class CraftingController : ICraftingController, IDisposable {
        private const string DataKey = "Crafting";
        private const float UpdatePeriod = 1f;

        private readonly IScheduler _scheduler;
        private readonly IResourceController _resourceController;
        private readonly IDataStorage _dataStorage;
        private readonly ITimeProvider _timeProvider;
        private readonly ListData<CraftingGroupData> _data;
        private readonly Dictionary<CraftingGroupConfig, CraftingGroup> _groups =
            new Dictionary<CraftingGroupConfig, CraftingGroup>();
        private readonly List<IDisposable> _groupUpdateTokens = new List<IDisposable>();

        [Inject]
        public CraftingController(IScheduler scheduler, IResourceController resourceController,
            IDataStorage dataStorage, ITimeProvider timeProvider) {
            _scheduler = scheduler;
            _resourceController = resourceController;
            _dataStorage = dataStorage;
            _timeProvider = timeProvider;
            _data = dataStorage.GetData<ListData<CraftingGroupData>>(DataKey);
        }

        public ICraftingGroup GetCraftingGroup(CraftingGroupConfig config) {
            return _groups.GetValue(config,
                () => {
                    var group = new CraftingGroup(config, _data.GetItem(config.ConfigId), _resourceController,
                        _timeProvider, Save);
                    _groupUpdateTokens.Add(_scheduler.SubscribeToPeriodicEvent(group.Update, UpdatePeriod));
                    return group;
                });
        }

        private void Save() {
            _dataStorage.SetData(_data, DataKey);
        }

        public void Dispose() {
            foreach (var token in _groupUpdateTokens) {
                token.Dispose();
            }
        }
    }
}