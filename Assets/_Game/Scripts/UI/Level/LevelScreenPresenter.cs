using _Game.Scripts.Data.Configs.Works;
using _Game.Scripts.Data.Configs.Works.UI;
using _Game.Scripts.DI;
using _Game.Scripts.Game.Level;
using _Game.Scripts.Game.Resource;
using GeneralUtils;

namespace _Game.Scripts.UI.Level {
    public class LevelScreenPresenter : UIPresenter<ILevelScreenView> {
        private readonly Event _closeEvent = new Event();

        private readonly ILevelController _levelController;
        private readonly IResourceController _resourceController;
        private readonly IContainer _container;
        private readonly LevelScreenParameters _parameters;

        [Inject]
        public LevelScreenPresenter(ILevelController levelController, IResourceController resourceController,
            IContainer container, LevelScreenParameters parameters) {
            _levelController = levelController;
            _resourceController = resourceController;
            _container = container;
            _parameters = parameters;
        }

        protected override void PerformOpen() {
            _levelController.Depth.Subscribe(View.SetDepth, true);
            _levelController.CollectDropEvent.Subscribe(OnCollectDrop);
            _resourceController.InventorySize.Subscribe(OnInventorySizeUpdate, true);

            _parameters.LevelWork.TryDoWithParameters(_container, (ref LevelWorkParameters parameters) => {
                parameters.CloseEvent = _closeEvent;
                parameters.OnCantCollectResource = View.DisplayCantCollectResource;
            });

            View.InitResourcePanels(_resourceController);
            View.ToggleResourcePanels(true);
            View.OpenInventoryEvent.Subscribe(OpenInventory);
            View.OpenLevelShopEvent.Subscribe(OpenLevelShop);
        }

        private void OnCollectDrop(IResourceValue value) {
            View.DisplayCollectResource(value.Value[0]);
        }

        private void OpenLevelShop() {
            View.ToggleResourcePanels(false);
            _parameters.ShopWork.TryDoWithParameters(_container, (ref ICloseParameters parameters) =>
                parameters.CloseCallback = () => View.ToggleResourcePanels(true));
        }

        private void OpenInventory() {
            View.ToggleResourcePanels(false);
            _parameters.InventoryWork.TryDoWithParameters(_container, (ref ICloseParameters parameters) =>
                parameters.CloseCallback = () => View.ToggleResourcePanels(true));
        }

        private void OnInventorySizeUpdate(int size) {
            var full = _resourceController.InventoryCapacity.Value is { } capacity && capacity <= size;
            View.SetInventoryFull(full);
        }

        protected override void PerformClose() {
            _levelController.Depth.Unsubscribe(View.SetDepth);
            _levelController.CollectDropEvent.Unsubscribe(OnCollectDrop);
            _resourceController.InventorySize.Unsubscribe(OnInventorySizeUpdate);
            _closeEvent.Invoke();

            View.OpenInventoryEvent.Unsubscribe(OpenInventory);
            View.OpenLevelShopEvent.Unsubscribe(OpenLevelShop);
        }
    }
}