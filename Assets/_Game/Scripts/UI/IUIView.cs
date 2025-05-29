using GeneralUtils;

namespace _Game.Scripts.UI
{
	public interface IUIView
	{
		IEvent CloseEvent { get; }
		IUpdatedValue<UIView.State> ViewState { get; }

		public void SetUIPointProvider(IUIPointProvider uiPointProvider);

		void Show();
		void Hide();
	}
}