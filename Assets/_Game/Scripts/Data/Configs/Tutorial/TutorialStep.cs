using System;
using _Game.Scripts.Data.Configs.Tutorial.StepFinishConditions;
using _Game.Scripts.Data.SerializedReference;
using _Game.Scripts.Tutorial;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.Data.Configs.Tutorial {
    [Serializable]
    public class TutorialStep {
        [SerializeReferenceMenu]
        [SerializeReference] private TutorialStepFinishCondition _finishCondition;
        public ITutorialStepFinishCondition FinishCondition => _finishCondition;

        [SerializeField] private Optional<TextData> _text;
        [CanBeNull] public TextData Text => _text.ToNullableClass();

        [SerializeField] private Optional<HiderData> _hider;
        [CanBeNull] public HiderData Hider => _hider.ToNullableClass();

        // step end condition

        // tutorial objects

        [Serializable]
        public class TextData {
            [SerializeField] private PositionData _position;
            public PositionData Position => _position;

            [TextArea] [SerializeField] private string _text;
            public string Text => _text;
        }

        [Serializable]
        public class HiderData {
            [SerializeField] private PositionData _position;
            public PositionData Position => _position;

            [SerializeField] private bool _clickThroughHole;
            public bool ClickThroughHole => _clickThroughHole;
        }

        [Serializable]
        public class PositionData {
            [SerializeField] private Optional<ReferenceElementPath> _referenceElementPath;
            [CanBeNull] public ReferenceElementPath ReferenceElementPath => _referenceElementPath.ToNullableClass();

            [SerializeField] private Vector2 _pivot = Vector2.one / 2f;
            public Vector2 Pivot => _pivot;

            [SerializeField] private Vector2 _anchorMin = Vector2.one / 2f;
            public Vector2 AnchorMin => _anchorMin;

            [SerializeField] private Vector2 _anchorMax = Vector2.one / 2f;
            public Vector2 AnchorMax => _anchorMax;

            [SerializeField] private Optional<Size> _sizeOverride;
            [CanBeNull] public Size SizeOverride => _sizeOverride.ToNullableClass();

            [SerializeField] private Vector2 _position;
            public Vector2 Position => _position;

            [Serializable]
            public class Size {
                [SerializeField] private Vector2 _value;
                public Vector2 Value => _value;

                [SerializeField] private bool _useAsOffset;
                public bool UseAsOffset => _useAsOffset;
            }
        }
    }
}