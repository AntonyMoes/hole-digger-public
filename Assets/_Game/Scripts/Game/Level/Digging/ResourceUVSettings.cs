using System.Linq;
using _Game.Scripts.Game.Level.DynamicTerrain;
using UnityEngine;

namespace _Game.Scripts.Game.Level.Digging {
    public class ResourceUVSettings : IUVSettings {
        public (Vector2 origin, Vector2 right, Vector2 up, Vector2 sideSize)[] UVData => new[] {
            new Vector2(0.375f, 0.5f),
            new Vector2(0.625f, 0.5f),
            new Vector2(0.375f, 0),
            new Vector2(0.125f, 0.5f),
            new Vector2(0.375f, 0.75f),
            new Vector2(0.375f, 0.25f),
        }.Select(origin => (origin, Vector2.right, Vector2.up, Vector2.one * 0.25f)).ToArray();

        public Vector2 UVSideSize => Vector2.one * 0.25f;
    }
}