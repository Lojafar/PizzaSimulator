using Cysharp.Threading.Tasks;
using Game.Root.AssetsManagment;
using UnityEngine;
namespace Game.PizzeriaSimulator.Player.Input.Factory
{
    class PCPlayerInputFactory : IPlayerInputFactory
    {
        readonly IAssetsProvider assetsProvider;
        readonly Transform canvasParentTransform;
        PCPlayerInput inputPrefab;
        public PCPlayerInputFactory(IAssetsProvider _assetsProvider, Transform _canvasParentTransform)
        {
            assetsProvider = _assetsProvider;
            canvasParentTransform = _canvasParentTransform;
        }
        public async UniTask Prewarm()
        {
            inputPrefab = await assetsProvider.LoadAsset<PCPlayerInput>(AssetsKeys.PCPlayerInputPrefab);
        }
        public IPlayerInput CreateInput()
        {
            PCPlayerInput input = Object.Instantiate(inputPrefab, canvasParentTransform);
            return input;
        }
    }
}
