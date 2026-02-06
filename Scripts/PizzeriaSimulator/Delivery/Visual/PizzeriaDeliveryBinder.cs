using Cysharp.Threading.Tasks;
using Game.Root.AssetsManagment;
using Game.Root.ServicesInterfaces;
using UnityEngine;
using Zenject;

namespace Game.PizzeriaSimulator.Delivery.Visual
{
    using DeviceType = Game.Root.User.Environment.DeviceType;
    class PizzeriaDeliveryBinder
    {
        readonly PizzeriaDelivery pizzeriaDelivery;
        readonly IAssetsProvider assetsProvider;
        readonly Transform uiParent;
        readonly DeviceType deviceType;
        readonly DiContainer diContainer;
        public PizzeriaDeliveryBinder(PizzeriaDelivery _pizzeriaDelivery, DeviceType _deviceType, IAssetsProvider _assetsProvider, Transform _uiParent, DiContainer _diContainer) 
        {
            pizzeriaDelivery = _pizzeriaDelivery;
            deviceType = _deviceType;
            assetsProvider = _assetsProvider;
            uiParent = _uiParent;
            diContainer = _diContainer;
        }
        public async UniTask Bind()
        {
            PizzeriaDeliveryViewBase viewPrefab = await assetsProvider.LoadAsset<PizzeriaDeliveryViewBase>(AssetsKeys.PizzeriaDeliveryView);
            PizzeriaDeliveryViewBase view = Object.Instantiate(viewPrefab, uiParent);
            PizzeriaDeliveryVM pizzeriaDeliveryVM = new(pizzeriaDelivery, deviceType);
            view.Bind(pizzeriaDeliveryVM);
            pizzeriaDeliveryVM.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(pizzeriaDeliveryVM);
        }
    }
}
