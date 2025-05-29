using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data.Configs.Meta;
using _Game.Scripts.Game.Resource;
using _Game.Scripts.Time;

namespace _Game.Scripts.Game.Crafting {
    public class CraftingGroup : ICraftingGroup {
        private readonly CraftingGroupConfig _config;
        private readonly CraftingGroupData _craftingGroup;
        private readonly ITimeProvider _timeProvider;
        private readonly Action _save;

        public IReadOnlyList<CraftingConfig> Recipes => _config.Recipes;
        private readonly IReadOnlyList<Crafter> _crafters;
        public IReadOnlyList<ICrafter> Crafters => _crafters;

        public CraftingGroup(CraftingGroupConfig config, CraftingGroupData craftingGroup,
            IResourceController resourceController, ITimeProvider timeProvider, Action save) {
            _config = config;
            _craftingGroup = craftingGroup;
            _timeProvider = timeProvider;
            _save = save;

            _crafters = Enumerable.Range(0, config.CrafterCount)
                .Select(i => GetCrafter(i, resourceController))
                .ToArray();
        }

        private Crafter GetCrafter(int index, IResourceController resourceController) {
            while (_craftingGroup.processes.Count <= index) {
                _craftingGroup.processes.Add(new CraftingGroupData.CraftingProcess());
            }

            var process = _craftingGroup.processes[index];
            return new Crafter(Recipes, process, resourceController, _timeProvider, _save);
        }

        public void Update() {
            foreach (var crafter in _crafters) {
                crafter.Update();
            }
        }
    }
}