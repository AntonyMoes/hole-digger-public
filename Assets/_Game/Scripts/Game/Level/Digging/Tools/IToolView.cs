using System;
using System.Collections.Generic;
using _Game.Scripts.Data.Configs;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.Game.Level.Digging.Tools {
    public interface IToolView : IDisposable {
        public void Init(ITool tool, IEvent prefabUpdateEvent, Func<GameObject> prefabProvider, IEnumerable<SoundConfig> sounds);
        
        public void Play(Vector3 cellPosition, Vector3 impactPoint, float heightRatio);
        public void Stop();
    }
}