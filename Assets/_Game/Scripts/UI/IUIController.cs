using System;
using GeneralUtils;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Game.Scripts.UI {
    public interface IUIController : IDisposable {
        public IUpdatedValue<GameObject> ActiveView { get; }

        public bool Open<TView>(GameObject uiPrefab, IUIPresenter<TView> presenter, UILayer uiLayer,
            [CanBeNull] Action closeCallback)
            where TView : IUIView;

        public TObject CreateTutorialObject<TObject>(TObject prefab) where TObject : Object;
    }
}