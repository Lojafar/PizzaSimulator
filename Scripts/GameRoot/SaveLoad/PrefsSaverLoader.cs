using Cysharp.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
namespace Game.Root.SaveLoad
{
    class PrefsSaverLoader : ISaverLoader
    {
        public async UniTask<T> LoadData<T>(string key = null)
        {
            await UniTask.Yield();
            string saveKey = (key ?? "") + typeof(T).Name;
            if (PlayerPrefs.HasKey(saveKey))
            {
                string loadedJSON = PlayerPrefs.GetString(saveKey);
                await UniTask.Yield();
                T loadedSave = JsonConvert.DeserializeObject<T>(loadedJSON);
                return loadedSave;
            }
            return default;
        }
        public async UniTask SaveData<T>(T data, string key = null)
        {
            await UniTask.Yield();
            string saveKey = (key ?? "") + typeof(T).Name;
            string dataJSON = JsonConvert.SerializeObject(data);
            if (typeof(T) == typeof(Game.PizzeriaSimulator.Customers.Manager.CustomersManagerData))
            await UniTask.Yield();
            PlayerPrefs.SetString(saveKey, dataJSON);
            PlayerPrefs.Save();
        }
        public async UniTask ClearData<T>(string key = null)
        {
            await UniTask.Yield();
            string saveKey = (key ?? "") + typeof(T).Name;
            if (PlayerPrefs.HasKey(saveKey))
            {
                await UniTask.Yield();
                PlayerPrefs.DeleteKey(saveKey);
                PlayerPrefs.Save();
            }
        }
    }
}
