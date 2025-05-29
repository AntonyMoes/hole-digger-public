namespace _Game.Scripts.Audio {
    public interface IAudioHandler {
        public bool Mute { get; set; }
        public float Volume { get; set; }

        public void Pause(bool pause);
        public void Stop(float time = 0f);
    }
}