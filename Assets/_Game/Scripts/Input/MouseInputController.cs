using System;
using System.Collections.Generic;
using _Game.Scripts.DI;
using _Game.Scripts.Scheduling;
using _Game.Scripts.Utils;
using GeneralUtils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Game.Scripts.Input {
    public class MouseInputController : IInputController, IDisposable {
        private readonly ITimeEventProvider _scheduler;
        private readonly EventSystem _eventSystem;
        private readonly LayerMask _uiLayer;

        private readonly List<RaycastResult> _raycastBuffer = new List<RaycastResult>();

        private readonly Event<Vector2, GameObject> _uiTapEvent = new Event<Vector2, GameObject>();
        public IEvent<Vector2, GameObject> UITapEvent => _uiTapEvent;

        private readonly Event<Vector2> _nonUITapEvent = new Event<Vector2>();
        public IEvent<Vector2> NonUITapEvent => _nonUITapEvent;

        private bool _hadClick;

        [Inject]
        public MouseInputController(ITimeEventProvider scheduler, EventSystem eventSystem, LayerMask uiLayer) {
            _scheduler = scheduler;
            scheduler.FrameEvent.Subscribe(ProcessFrame);
            _eventSystem = eventSystem;
            _uiLayer = uiLayer;
        }

        public void ProcessFrame(float deltaTime) {
            var frame = UnityEngine.Time.frameCount;

            if (UnityEngine.Input.GetMouseButtonDown(0)) {
                _hadClick = false;
                var uiTap = false;
                var mousePosition = UnityEngine.Input.mousePosition;
                foreach (var cast in GetSystemRaycasts(mousePosition)) {
                    if (_uiLayer.Contains(cast.gameObject.layer)) {
                        uiTap = true;
                        break;
                    }
                }

                if (!uiTap) {
                    _nonUITapEvent.Invoke(mousePosition);
                    _hadClick = true;
                }
            }

            if (UnityEngine.Input.GetMouseButtonUp(0) && !_hadClick) {
                var mousePosition = UnityEngine.Input.mousePosition;
                foreach (var cast in GetSystemRaycasts(mousePosition)) {
                    if (_uiLayer.Contains(cast.gameObject.layer)) {
                        _uiTapEvent.Invoke(mousePosition, cast.gameObject);
                        return;
                    }
                }

                _uiTapEvent.Invoke(mousePosition, null);
            }
        }

        private IReadOnlyList<RaycastResult> GetSystemRaycasts(Vector2 mousePosition) {
            var eventData = new PointerEventData(_eventSystem) { position = mousePosition };
            _eventSystem.RaycastAll(eventData, _raycastBuffer);
            return _raycastBuffer;
        }

        public void Dispose() {
            _scheduler.FrameEvent.Unsubscribe(ProcessFrame);
        }
    }
}