using Cysharp.Threading.Tasks;
using UnityEngine;
namespace Game.Root.AssetsManagment
{
    public interface IAssetsProvider
    {
        public UniTask<T> LoadAsset<T>(string Key) where T : Object;
    }
}
