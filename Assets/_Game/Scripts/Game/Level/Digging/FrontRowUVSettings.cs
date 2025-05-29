using System.Linq;
using _Game.Scripts.Game.Level.DynamicTerrain;
using _Game.Scripts.Utils;
using UnityEngine;

namespace _Game.Scripts.Game.Level.Digging {
    public class FrontRowUVSettings : IUVSettings {
        public (Vector2 origin, Vector2 right, Vector2 up, Vector2 sideSize)[] UVData { get; }

        public FrontRowUVSettings(Vector3Int meshSizeInCells, Vector3Int cellPosition) {
            var size = (Vector2) ((Vector3) meshSizeInCells).Inverted();
            var start = (Vector2) Vector3.Scale(cellPosition,size);

            UVData = Enumerable.Empty<(Vector2 origin, Vector2 right, Vector2 up, Vector2 sideSize)>()
                .Append((start, Vector2.right, Vector2.up, size))
                .Concat(Enumerable.Repeat((Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero), 5))
                .ToArray();
        }
    }
}