using Game.PizzeriaSimulator.Interactions.Interactor;
using Game.PizzeriaSimulator.OrdersHandle;
using Game.PizzeriaSimulator.OrdersHandle.Visual;
using Game.PizzeriaSimulator.PaymentReceive;
using Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor;
using Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor.Visual;
using Game.PizzeriaSimulator.PaymentReceive.Visual;
using Game.PizzeriaSimulator.PizzaCreation;
using Game.PizzeriaSimulator.PizzaCreation.Config;
using Game.PizzeriaSimulator.PizzaCreation.Visual;
using Game.PizzeriaSimulator.PizzaHold;
using Game.PizzeriaSimulator.PizzaHold.Visual;
using Game.PizzeriaSimulator.Player.CameraController;
using Game.PizzeriaSimulator.Player.Handler;
using Game.PizzeriaSimulator.Player.Input;
using Game.PizzeriaSimulator.Player.Input.Factory;
using Game.PizzeriaSimulator.PlayerSpawner;
using Game.Root.AssetsManagment;
using Game.Root.ServicesInterfaces;
using Game.Root.User.Environment;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Game.PizzeriaSimulator.Entry
{
    class PizzeriaEntryPoint : IInitializable
    {
        readonly PizzeriaSceneReferences sceneReferences;
        readonly IAssetsProvider assetsProvider;
        readonly DeviceType deviceType;
        readonly DiContainer diContainer;
        PaymentReceiver paymentReceiver;
        PizzaHolder pizzaHolder;
        PizzaCreator pizzaCreator;
        PizzeriaOrdersHandler ordersHandler;
        public PizzeriaEntryPoint(PizzeriaSceneReferences _sceneReferences, DiContainer _diContainer) 
        {
            sceneReferences = _sceneReferences;
            assetsProvider = _diContainer.Resolve<IAssetsProvider>();
            deviceType = _diContainer.Resolve<IEnvironmentHandler>().DeviceType;
            diContainer = _diContainer;
        }
        public void Initialize()
        {
            Entry().Forget();
        }
        public async UniTaskVoid Entry()
        {
            IPlayerInput playerInput = (await GetInputFactory()).CreateInput();
            diContainer.Bind<IPlayerInput>().FromInstance(playerInput).AsSingle();


            PizzeriaPlayerSpawner playerSpawner = new(playerInput, sceneReferences, diContainer.Resolve<IAssetsProvider>(), diContainer);
            await playerSpawner.SpawnPlayer();

            PlayerCameraControllerBase cameraController = diContainer.Resolve<PlayerCameraControllerBase>();

            PizzaPlayerHandler playerHandler = new(playerInput, cameraController);
            playerHandler.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(playerHandler);

            paymentReceiver = new();
            diContainer.Bind<PaymentReceiver>().FromInstance(paymentReceiver).AsSingle();
            paymentReceiver.Init();

            PizzaCreatorConfig pizzaCreatorConfig = (await assetsProvider.LoadAsset<PizzaCreatorConfigSO>(AssetsKeys.PizzaCreatorConfig)).PizzaCreatorConfig;

            pizzaHolder = new PizzaHolder(pizzaCreatorConfig);

            pizzaCreator = new PizzaCreator(pizzaHolder, pizzaCreatorConfig);

            ordersHandler = new PizzeriaOrdersHandler(pizzaHolder);
            ordersHandler.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(ordersHandler);

            sceneReferences.OrderGIver.Init(ordersHandler);

            Interactor interactor = new(playerInput, cameraController,  paymentReceiver, pizzaCreator, ordersHandler);
            interactor.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(interactor);

            await CreateVisual();
        }
        public async UniTask CreateVisual()
        {
            await UniTask.Yield();
            new PizzaHolderBinder(pizzaHolder, sceneReferences, diContainer).Bind();
            new PizzaCreatorBinder(sceneReferences, pizzaCreator, diContainer).Bind();
            await (new PizzeriaOrderHandlBinder(ordersHandler, pizzaCreator, assetsProvider, sceneReferences.SceneCanvas.transform, deviceType, diContainer)).Bind();
            new PaymentReceiveBinder(sceneReferences, paymentReceiver, diContainer).Bind();
            if(paymentReceiver.GetPaymentProccesorByType(PaymentType.Cash) is CashPaymentProccesor cashPaymentProccesor)
            {
                new CashPaymentProccesorBinder(sceneReferences, cashPaymentProccesor, diContainer).Bind();
            }
            if (paymentReceiver.GetPaymentProccesorByType(PaymentType.Card) is CardPaymentProccesor cardPaymentProccesor)
            {
                new CardPaymentProccesorBinder(sceneReferences, cardPaymentProccesor, diContainer).Bind();
            }
        }
        public async UniTask<IPlayerInputFactory> GetInputFactory()
        {
            IPlayerInputFactory inputFactory = null;
            switch (deviceType)
            {
                case DeviceType.PC:
                    inputFactory = new PCPlayerInputFactory(assetsProvider, sceneReferences.SceneCanvas.transform);
                    break;
                case DeviceType.Mobile:
                    inputFactory = new MobilePlayerInputFactory(assetsProvider, sceneReferences.SceneCanvas.transform);
                    break;
                default:
                    UnityEngine.Debug.LogError("Devive type not implemented for input");
                    break;
            }
            await inputFactory.Prewarm();
            return inputFactory;
        }
    }
}
