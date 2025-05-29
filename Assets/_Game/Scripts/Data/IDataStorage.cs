namespace _Game.Scripts.Data {
    public interface IDataStorage {
        public T GetData<T>(string key) where T : new();
        public void SetData<T>(T data, string key);

        public void ForceSave();
    }
}