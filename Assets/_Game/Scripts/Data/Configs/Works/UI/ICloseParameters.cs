using System;

namespace _Game.Scripts.Data.Configs.Works.UI {
    public interface ICloseParameters {
        public Action CloseCallback { get; set; }
    }
}