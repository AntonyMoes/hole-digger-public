using System;
using UnityEngine;

namespace _Game.Scripts.Game.Level.DynamicTerrain {
    public class DynamicTriangle {
        public readonly DynamicVertex A;
        public readonly DynamicVertex B;
        public readonly DynamicVertex C;

        public DynamicTriangle(DynamicVertex a, DynamicVertex b, DynamicVertex c) {
            A = a;
            B = b;
            C = c;
        }

        public Vector3 CalculateNormal(Func<DynamicVertex, Vector3> getPosition) {
            var aPos = getPosition(A);
            var bPos = getPosition(B);
            var cPos = getPosition(C);

            return Vector3.Cross(bPos - aPos, cPos - aPos).normalized;
        }
    }
}