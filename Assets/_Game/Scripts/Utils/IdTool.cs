using System;
using System.Security.Cryptography;
using System.Text;

namespace _Game.Scripts.Utils {
    public static class IdTool {
        public static int MakeId(string str) {
            using var algorithm = SHA256.Create();
            var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(str));
            return BitConverter.ToInt32(hash, 0);
        }
    }
}