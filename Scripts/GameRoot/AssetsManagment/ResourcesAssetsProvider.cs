using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Root.AssetsManagment
{
    class ResourcesAssetsProvider : IAssetsProvider
    {
        public async UniTask<T> LoadAsset<T>(string Key) where T : Object
        {
            T loadedAsset = (T)(await Resources.LoadAsync<T>(Key));
            return loadedAsset;
        }
    }
}