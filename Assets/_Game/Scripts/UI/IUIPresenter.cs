using System;

namespace _Game.Scripts.UI {
    public interface IUIPresenter {
        IUIView UIView { get; }
        void Back();

        void Close();

        void Hide();
        void Show();
    }
    
    public interface IUIPresenter<in TView> : IUIPresenter where TView : IUIView {
        void Open(TView view, Action closeCallback);
    }
}