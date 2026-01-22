using Cysharp.Threading.Tasks;

namespace Game.Root.SaveLoad
{
    public interface ISaverLoader
    {
        public UniTask<T> LoadData<T>(string key = null);
        public UniTask SaveData<T>(T data, string key = null);
        public UniTask ClearData<T>(string key = null);
    }
}
