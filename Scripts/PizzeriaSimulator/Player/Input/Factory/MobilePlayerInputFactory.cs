using Cysharp.Threading.Tasks;
using Game.Root.AssetsManagment;
using UnityEngine;

namespace Game.PizzeriaSimulator.Player.Input.Factory
{
    class MobilePlayerInputFactory : IPlayerInputFactory
    {
        readonly IAssetsProvider assetsProvider;
        readonly Transform canvasParentTransform;
        MobilePlayerInput inputPrefab;
        public MobilePlayerInputFactory(IAssetsProvider _assetsProvider, Transform _canvasParentTransform)
        {
            assetsProvider = _assetsProvider;
            canvasParentTransform = _canvasParentTransform;
        }
        public async UniTask Prewarm()
        {
            inputPrefab = await assetsProvider.LoadAsset<MobilePlayerInput>(AssetsKeys.MobilePlayerInputPrefab);
        }
        public IPlayerInput CreateInput()
        {
            MobilePlayerInput input = Object.Instantiate(inputPrefab, canvasParentTransform);
            return input;
        }
    }
}
