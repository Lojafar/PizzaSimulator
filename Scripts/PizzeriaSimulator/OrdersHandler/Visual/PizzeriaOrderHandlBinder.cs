using Cysharp.Threading.Tasks;
using Game.PizzeriaSimulator.PizzaCreation;
using Game.Root.AssetsManagment;
using Game.Root.ServicesInterfaces;
using UnityEngine;
using Zenject;

namespace Game.PizzeriaSimulator.OrdersHandle.Visual
{
    class PizzeriaOrderHandlBinder
    {
        readonly PizzeriaOrdersHandler ordersHandler;
        readonly PizzaCreator pizzaCreator;
        readonly IAssetsProvider assetsProvider;
        readonly Transform uiParent;
        readonly DiContainer diContainer;
        public PizzeriaOrderHandlBinder( PizzeriaOrdersHandler _ordersHandler, PizzaCreator _pizzaCreator, IAssetsProvider _assetsProvider, Transform _uiParent, DiContainer _diContainer)
        {
            ordersHandler = _ordersHandler;
            pizzaCreator = _pizzaCreator;
            assetsProvider = _assetsProvider;
            uiParent = _uiParent;
            diContainer = _diContainer;
        }
        public async UniTask Bind()
        {
            PizzeriaOrderHandlViewBase pizzeriaOrderHandlUI = await assetsProvider.LoadAsset<PizzeriaOrderHandlViewBase>(AssetsKeys.PizzeriaOrdersUIView);
            PizzeriaOrderHandlViewBase spawnedUIView = Object.Instantiate(pizzeriaOrderHandlUI, uiParent);

            PizzeriaOrderHandlVM ordersHandlVM = new(ordersHandler, pizzaCreator);
            spawnedUIView.Bind(ordersHandlVM);
            ordersHandlVM.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(ordersHandlVM);
        }
    }
}
