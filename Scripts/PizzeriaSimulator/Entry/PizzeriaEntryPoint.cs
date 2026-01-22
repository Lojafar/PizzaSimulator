using Cysharp.Threading.Tasks;
using Game.PizzeriaSimulator.Boxes.Carry;
using Game.PizzeriaSimulator.Boxes.Manager;
using Game.PizzeriaSimulator.Computer;
using Game.PizzeriaSimulator.Computer.App.Market;
using Game.PizzeriaSimulator.Computer.App.Market.Visual;
using Game.PizzeriaSimulator.Computer.Visual;
using Game.PizzeriaSimulator.Customers.Manager;
using Game.PizzeriaSimulator.Customers.OrdersProcces;
using Game.PizzeriaSimulator.Customers.SettingsConfig;
using Game.PizzeriaSimulator.Delivery;
using Game.PizzeriaSimulator.Delivery.Config;
using Game.PizzeriaSimulator.Delivery.Visual;
using Game.PizzeriaSimulator.Interactions.Interactor;
using Game.PizzeriaSimulator.OrdersHandle;
using Game.PizzeriaSimulator.OrdersHandle.Visual;
using Game.PizzeriaSimulator.PaymentReceive;
using Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor;
using Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor.Visual;
using Game.PizzeriaSimulator.PaymentReceive.Visual;
using Game.PizzeriaSimulator.PaymentReceive.Visual.Config;
using Game.PizzeriaSimulator.PizzaCreation;
using Game.PizzeriaSimulator.PizzaCreation.Config;
using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold;
using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold.Visual;
using Game.PizzeriaSimulator.PizzaCreation.Visual;
using Game.PizzeriaSimulator.PizzaHold;
using Game.PizzeriaSimulator.PizzaHold.Visual;
using Game.PizzeriaSimulator.PizzasConfig;
using Game.PizzeriaSimulator.Player.CameraController;
using Game.PizzeriaSimulator.Player.Handler;
using Game.PizzeriaSimulator.Player.Input;
using Game.PizzeriaSimulator.Player.Input.Factory;
using Game.PizzeriaSimulator.PlayerSpawner;
using Game.PizzeriaSimulator.SaveLoadHelp;
using Game.PizzeriaSimulator.UI.StatusPanel;
using Game.Root.AssetsManagment;
using Game.Root.ServicesInterfaces;
using Game.Root.User.Environment;
using System.Collections.Generic;
using Zenject;

namespace Game.PizzeriaSimulator.Entry
{
    class PizzeriaEntryPoint : IInitializable
    {
        readonly PizzeriaSceneReferences sceneReferences;
        readonly PizzeriaSaveLoadHelper saveLoadHelper;
        readonly IAssetsProvider assetsProvider;
        readonly DeviceType deviceType;
        readonly DiContainer diContainer;
        readonly List<IInittable> inittableServices;
        readonly List<ITaskInittable> taskInittableServices;
        PaymentReceiver paymentReceiver;
        PizzaHolder pizzaHolder;
        PizzaIngredientsHolder pizzaIngredientsHolder;
        PizzaCreator pizzaCreator;
        PizzeriaOrdersHandler ordersHandler;
        PizzeriaDelivery pizzeriaDelivery;
        PizzeriaComputer pizzeriaComputer;
        const int approxServicCount = 10;
        const int approxTaskServicCount = 3;
        public PizzeriaEntryPoint(PizzeriaSceneReferences _sceneReferences, PizzeriaSaveLoadHelper _saveLoadHelper, DiContainer _diContainer) 
        {
            sceneReferences = _sceneReferences;
            saveLoadHelper = _saveLoadHelper;
            assetsProvider = _diContainer.Resolve<IAssetsProvider>();
            deviceType = _diContainer.Resolve<IEnvironmentHandler>().DeviceType;
            diContainer = _diContainer;
            inittableServices = new List<IInittable>(approxServicCount);
            taskInittableServices = new List<ITaskInittable>(approxTaskServicCount);
        }
        public void Initialize()
        {
            Entry().Forget();
        }
        public async UniTaskVoid Entry()
        {
            await saveLoadHelper.LoadAndBindSaves();

            IPlayerInput playerInput = (await GetInputFactory()).CreateInput();
            diContainer.Bind<IPlayerInput>().FromInstance(playerInput).AsSingle();

            PizzeriaPlayerSpawner playerSpawner = new(playerInput, sceneReferences, assetsProvider, diContainer);
            await playerSpawner.SpawnPlayer();
            PlayerCameraControllerBase cameraController = diContainer.Resolve<PlayerCameraControllerBase>();

            Interactor interactor = new(playerInput, cameraController);
            inittableServices.Add(interactor);

            PizzaPlayerHandler playerHandler = new(playerInput, cameraController);
            inittableServices.Add(playerHandler);

            BoxesCarrier boxesCarrier = new(interactor, playerInput, cameraController, sceneReferences, diContainer);
            inittableServices.Add(boxesCarrier);


            AllPizzaConfig allPizzaConfig = (await assetsProvider.LoadAsset<AllPizzaConfigSO>(AssetsKeys.AllPizzaConfig)).AllPizzaConfig;
            PizzaCreatorConfig pizzaCreatorConfig = (await assetsProvider.LoadAsset<PizzaCreatorConfigSO>(AssetsKeys.PizzaCreatorConfig)).PizzaCreatorConfig;
            PaymentVisualConfig paymentVisualConfig = (await assetsProvider.LoadAsset<PaymentVisualConfigSO>(AssetsKeys.PaymentVisualConfig)).PaymentVisualConfig;
            diContainer.Bind<AllPizzaConfig>().FromInstance(allPizzaConfig).AsSingle();
            diContainer.Bind<PizzaCreatorConfig>().FromInstance(pizzaCreatorConfig).AsSingle();
            diContainer.Bind<PaymentVisualConfig>().FromInstance(paymentVisualConfig).AsSingle();

            PizzaHolderData pizzaHolderData = await saveLoadHelper.LoadData<PizzaHolderData>();
            pizzaHolder = new PizzaHolder(pizzaHolderData);
            diContainer.Bind<PizzaHolder>().FromInstance(pizzaHolder).AsSingle();
            inittableServices.Add(pizzaHolder);
            IngredientsHolderData ingredientsHolderData = await saveLoadHelper.LoadData<IngredientsHolderData>();
            pizzaIngredientsHolder = new PizzaIngredientsHolder(pizzaCreatorConfig, ingredientsHolderData);
            diContainer.Bind<PizzaIngredientsHolder>().FromInstance(pizzaIngredientsHolder).AsSingle();
            inittableServices.Add(pizzaIngredientsHolder);

            PizzaCreatorData pizzaCreatorData = await saveLoadHelper.LoadData<PizzaCreatorData>();
            pizzaCreator = new PizzaCreator(pizzaCreatorData, pizzaIngredientsHolder, pizzaHolder, boxesCarrier, interactor, allPizzaConfig, pizzaCreatorConfig);
            inittableServices.Add(pizzaCreator);
            diContainer.Bind<PizzaCreator>().FromInstance(pizzaCreator).AsSingle();

            ordersHandler = new PizzeriaOrdersHandler(pizzaHolder, interactor);
            inittableServices.Add(ordersHandler);

            PizzeriaDeliveryConfig deliveryConfig = (await assetsProvider.LoadAsset<PizzeriaDeliveryConfigSO>(AssetsKeys.PizzeriaDeliveryConfig)).DeliveryConfig;

            BoxesManager boxesManager = new (saveLoadHelper, boxesCarrier, deliveryConfig);
            taskInittableServices.Add(boxesManager);

            diContainer.Bind<PizzeriaDeliveryConfig>().FromInstance(deliveryConfig).AsSingle();
            pizzeriaDelivery = new PizzeriaDelivery(deliveryConfig, boxesManager, sceneReferences);
            diContainer.BindInterfacesAndSelfTo<PizzeriaDelivery>().FromInstance(pizzeriaDelivery).AsSingle();


            paymentReceiver = new(interactor, saveLoadHelper.PlayerWallet);
            diContainer.Bind<PaymentReceiver>().FromInstance(paymentReceiver).AsSingle();
            inittableServices.Add(paymentReceiver);

            pizzeriaComputer = new PizzeriaComputer(interactor, diContainer);
            inittableServices.Add(pizzeriaComputer);


            CustomersSettingsConfig customersSettingsConfig = (await assetsProvider.LoadAsset<CustomersSettingsConfigSO>(AssetsKeys.CustomersSettingsConfig)).CustomersSettingsConfig;

            CustomersOrdersProccesor customersOrdersProccesor = new(paymentReceiver, ordersHandler, allPizzaConfig, customersSettingsConfig);

            CustomersManagerData customersManagerData = await saveLoadHelper.LoadData<CustomersManagerData>();
            CustomersManager customersManager = new(customersManagerData, customersOrdersProccesor, ordersHandler, sceneReferences, assetsProvider, customersSettingsConfig);
            CustomerVisualManager customerVisualManager = new(assetsProvider, customersManager, customersOrdersProccesor, sceneReferences, paymentVisualConfig);
            diContainer.Bind<CustomersManager>().FromInstance(customersManager).AsSingle();
            taskInittableServices.Add(customerVisualManager);
            taskInittableServices.Add(customersManager);


            await CreateVisual();
            await HandleInittableServices();

            saveLoadHelper.StartFollowSaves();
        }
        public async UniTask CreateVisual()
        {
            new PizzaHolderBinder(pizzaHolder, sceneReferences, diContainer.Resolve<AllPizzaConfig>(), diContainer).Bind();
            new PizzaIngredientsHolderBinder(pizzaIngredientsHolder, sceneReferences, diContainer).Bind();
            new PizzaCreatorBinder(pizzaCreator, sceneReferences, diContainer).Bind();
            await (new PizzeriaOrderHandlBinder(ordersHandler, pizzaCreator, assetsProvider, sceneReferences.SceneCanvas.transform, deviceType, diContainer)).Bind();
            new PaymentReceiveBinder(sceneReferences, paymentReceiver, diContainer).Bind();
            if(paymentReceiver.GetPaymentProccesorByType(PaymentType.Cash) is CashPaymentProccesor cashPaymentProccesor)
            {
                new CashPaymentProccesorBinder(sceneReferences, cashPaymentProccesor, deviceType, diContainer).Bind();
            }
            if (paymentReceiver.GetPaymentProccesorByType(PaymentType.Card) is CardPaymentProccesor cardPaymentProccesor)
            {
                new CardPaymentProccesorBinder(sceneReferences, cardPaymentProccesor, diContainer).Bind();
            }
            new PizzeriaComputerBinder(pizzeriaComputer, sceneReferences, diContainer).Bind();
            if(pizzeriaComputer.GetAppByType(Computer.App.ComputerAppType.Market) is MarketCompApp marketApp)
            {
                new MarketCompAppBinder(marketApp, sceneReferences, diContainer).Bind();
            }

            StatusPanelModel statusPanelModel = new(saveLoadHelper.PlayerWallet);
            inittableServices.Add(statusPanelModel);
            await new StatusPanelBinder(statusPanelModel, assetsProvider, sceneReferences.SceneCanvas.transform, diContainer).Bind();

            await (new PizzeriaDeliveryBinder(pizzeriaDelivery, assetsProvider, sceneReferences.SceneCanvas.transform, diContainer)).Bind();
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
        async UniTask HandleInittableServices()
        {
            foreach(IInittable inittableService in inittableServices)
            {
                inittableService.Init();
                if(inittableService is ISceneDisposable disposable)
                {
                    diContainer.Bind<ISceneDisposable>().FromInstance(disposable);
                }
            }
            foreach(ITaskInittable taskInittable in taskInittableServices)
            {
                await taskInittable.Init();
                if (taskInittable is ISceneDisposable disposable)
                {
                    diContainer.Bind<ISceneDisposable>().FromInstance(disposable);
                }
            }
            inittableServices.Clear();
        }
    }
}
