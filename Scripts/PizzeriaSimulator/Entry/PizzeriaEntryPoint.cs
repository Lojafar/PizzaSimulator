using Assets.Game.Scripts.PizzeriaSimulator.UI.NewLevelPopUp;
using Assets.Game.Scripts.PizzeriaSimulator.UI.NewLevelPopUp.Visual;
using Cysharp.Threading.Tasks;
using Game.PizzeriaSimulator.Boxes.Carry;
using Game.PizzeriaSimulator.Boxes.Carry.Visual;
using Game.PizzeriaSimulator.Boxes.Manager;
using Game.PizzeriaSimulator.Computer;
using Game.PizzeriaSimulator.Computer.App.ManagmentApp;
using Game.PizzeriaSimulator.Computer.App.ManagmentApp.Visual;
using Game.PizzeriaSimulator.Computer.App.Market;
using Game.PizzeriaSimulator.Computer.App.Market.Visual;
using Game.PizzeriaSimulator.Computer.Visual;
using Game.PizzeriaSimulator.Customers.Manager;
using Game.PizzeriaSimulator.Customers.OrdersProcces;
using Game.PizzeriaSimulator.Customers.SettingsConfig;
using Game.PizzeriaSimulator.DayCycle.Config;
using Game.PizzeriaSimulator.DayCycle.Manager;
using Game.PizzeriaSimulator.DayCycle.Visual;
using Game.PizzeriaSimulator.Delivery;
using Game.PizzeriaSimulator.Delivery.Config;
using Game.PizzeriaSimulator.Delivery.Visual;
using Game.PizzeriaSimulator.Interactions.Interactor;
using Game.PizzeriaSimulator.Levels.Config;
using Game.PizzeriaSimulator.Levels.Handler;
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
using Game.PizzeriaSimulator.Pizzeria.Furniture.Config;
using Game.PizzeriaSimulator.Pizzeria.Furniture.Placement.Manager;
using Game.PizzeriaSimulator.Pizzeria.Managment;
using Game.PizzeriaSimulator.Pizzeria.Managment.Config;
using Game.PizzeriaSimulator.Pizzeria.Managment.Visual;
using Game.PizzeriaSimulator.Player;
using Game.PizzeriaSimulator.Player.CameraController;
using Game.PizzeriaSimulator.Player.Handler;
using Game.PizzeriaSimulator.Player.Input;
using Game.PizzeriaSimulator.Player.Input.Factory;
using Game.PizzeriaSimulator.PlayerSpawner;
using Game.PizzeriaSimulator.SaveLoadHelp;
using Game.PizzeriaSimulator.Tutorial;
using Game.PizzeriaSimulator.Tutorial.Visual;
using Game.PizzeriaSimulator.UI.EndDayStatsPanel;
using Game.PizzeriaSimulator.UI.EndDayStatsPanel.Visual;
using Game.PizzeriaSimulator.UI.StatusPanel;
using Game.PizzeriaSimulator.UI.StatusPanel.Visual;
using Game.Root.AssetsManagment;
using Game.Root.ServicesInterfaces;
using Game.Root.User.Environment;
using System.Collections.Generic;
using System.Linq;
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
        readonly List<IPrewarmable> prewarmableServices;
        readonly List<IInittable> inittableServices;
        DayCycleManager dayCycleManager;
        PizzeriaManager pizzeriaManager;
        BoxesCarrier boxesCarrier;
        PaymentReceiver paymentReceiver;
        PizzaHolder pizzaHolder;
        PizzaIngredientsHolder pizzaIngredientsHolder;
        PizzaCreator pizzaCreator;
        PizzeriaOrdersHandler ordersHandler;
        PizzeriaDelivery pizzeriaDelivery;
        PizzeriaComputer pizzeriaComputer;
        TutorialController tutorialController;
        const int approxPrewarmServices = 3;
        const int approxServicCount = 10;
        public PizzeriaEntryPoint(PizzeriaSceneReferences _sceneReferences, PizzeriaSaveLoadHelper _saveLoadHelper, DiContainer _diContainer) 
        {
            sceneReferences = _sceneReferences;
            saveLoadHelper = _saveLoadHelper;
            assetsProvider = _diContainer.Resolve<IAssetsProvider>();
            deviceType = _diContainer.Resolve<IEnvironmentHandler>().DeviceType;
            diContainer = _diContainer;
            prewarmableServices = new List<IPrewarmable>(approxPrewarmServices);
            inittableServices = new List<IInittable>(approxServicCount);
        }
        public void Initialize()
        {
            Entry().Forget();
        }
        public async UniTaskVoid Entry()
        {
            await saveLoadHelper.Prepare();
            await saveLoadHelper.LoadAndBindSaves();

            IPlayerInput playerInput = (await GetInputFactory()).CreateInput();
            diContainer.Bind<IPlayerInput>().FromInstance(playerInput).AsSingle();
            playerInput.Activate(false);

            PizzeriaPlayerSpawner playerSpawner = new(playerInput, sceneReferences, assetsProvider, diContainer);
            PizzaPlayer player = await playerSpawner.SpawnPlayer();

            PlayerCameraControllerBase cameraController = diContainer.Resolve<PlayerCameraControllerBase>();

            Interactor interactor = new(playerInput, cameraController);
            inittableServices.Add(interactor);
            diContainer.Bind<Interactor>().FromInstance(interactor).AsSingle();

            DayCycleConfig dayCycleConfig = (await assetsProvider.LoadAsset<DayCycleConfigSO>(AssetsKeys.DayCycleConfig)).DayCycleConfig;
            DayCycleManagerData dayCycleManagerData = await saveLoadHelper.LoadData<DayCycleManagerData>();
            dayCycleManager = new DayCycleManager(dayCycleManagerData, dayCycleConfig);
            diContainer.Bind<DayCycleManager>().FromInstance(dayCycleManager).AsSingle();
            inittableServices.Add(dayCycleManager);

            DayCycleVisualController dayCycleVisualController = new(dayCycleManager, dayCycleConfig, sceneReferences.DirectionalLight);
            inittableServices.Add(dayCycleVisualController);

            PizzaPlayerHandler playerHandler = new(playerInput, cameraController, player, dayCycleManager, sceneReferences);
            inittableServices.Add(playerHandler);

            boxesCarrier = new(interactor, playerInput, cameraController, sceneReferences, diContainer);
            inittableServices.Add(boxesCarrier);

            PizzeriaManagerData pizzeriaManagerData = await saveLoadHelper.LoadData<PizzeriaManagerData>();
            LevelsConfig levelsConfig = (await assetsProvider.LoadAsset<LevelsConfigSO>(AssetsKeys.LevelsConfig)).LevelsConfig;
            PizzeriaManagmentConfig managmentConfig = (await assetsProvider.LoadAsset<PizzeriaManagmentConfigSO>(AssetsKeys.PizzeriaManagmentConfig)).ManagmentConfig;
            pizzeriaManager = new(pizzeriaManagerData, sceneReferences.PizzeriaExpansionsContainer, managmentConfig, levelsConfig, diContainer);
            diContainer.Bind<PizzeriaManager>().FromInstance(pizzeriaManager).AsSingle();
            diContainer.Bind<LevelsConfig>().FromInstance(levelsConfig).AsSingle();
            diContainer.Bind<PizzeriaManagmentConfig>().FromInstance(managmentConfig).AsSingle();
            inittableServices.Add(pizzeriaManager);
            LevelsHandler levelsHandler = new(pizzeriaManager, levelsConfig, diContainer);
            inittableServices.Add(levelsHandler);

            PizzeriaFurnitureConfig pizzeriaFurnitureConfig = (await assetsProvider.LoadAsset<PizzeriaFurnitureConfigSO>(AssetsKeys.PizzeriaFurnitureConfig)).FurnitureConfig;
            PizzeriaFurnitureManagerData furnitureManagerData = await saveLoadHelper.LoadOrTryGetInitData<PizzeriaFurnitureManagerData>();
            PizzeriaFurnitureManager pizzeriaFurnitureManager = new(furnitureManagerData, pizzeriaManager, sceneReferences.FurniturePlaceAreaHolder, 
                sceneReferences.PizzeriaExpansionsContainer,  pizzeriaFurnitureConfig);
            diContainer.Bind<PizzeriaFurnitureConfig>().FromInstance(pizzeriaFurnitureConfig).AsSingle();
            diContainer.Bind<PizzeriaFurnitureManager>().FromInstance(pizzeriaFurnitureManager).AsSingle();
            inittableServices.Add(pizzeriaFurnitureManager);

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
            prewarmableServices.Add(boxesManager);
            inittableServices.Add(boxesManager);

            diContainer.Bind<PizzeriaDeliveryConfig>().FromInstance(deliveryConfig).AsSingle();
            pizzeriaDelivery = new PizzeriaDelivery(deliveryConfig, boxesManager, pizzeriaManager, saveLoadHelper.PlayerWallet, sceneReferences);
            diContainer.BindInterfacesAndSelfTo<PizzeriaDelivery>().FromInstance(pizzeriaDelivery).AsSingle();


            paymentReceiver = new(interactor, saveLoadHelper.PlayerWallet);
            diContainer.Bind<PaymentReceiver>().FromInstance(paymentReceiver).AsSingle();
            inittableServices.Add(paymentReceiver);

            CustomersSettingsConfig customersSettingsConfig = (await assetsProvider.LoadAsset<CustomersSettingsConfigSO>(AssetsKeys.CustomersSettingsConfig)).CustomersSettingsConfig;

            CustomersOrdersProccesor customersOrdersProccesor = new(paymentReceiver, ordersHandler, allPizzaConfig, customersSettingsConfig);

            CustomersManagerData customersManagerData = await saveLoadHelper.LoadData<CustomersManagerData>();
            CustomersManager customersManager = new(customersManagerData, dayCycleManager, pizzeriaManager, customersOrdersProccesor, ordersHandler, sceneReferences, assetsProvider, customersSettingsConfig);
            CustomerVisualManager customerVisualManager = new(assetsProvider, customersManager, customersOrdersProccesor, sceneReferences, paymentVisualConfig);
            diContainer.Bind<CustomersManager>().FromInstance(customersManager).AsSingle();
            prewarmableServices.Add(customersManager);
            prewarmableServices.Add(customerVisualManager);
            inittableServices.Add(customersManager);
            inittableServices.Add(customerVisualManager);

            pizzeriaComputer = new PizzeriaComputer(interactor, diContainer);
            inittableServices.Add(pizzeriaComputer);

            tutorialController = new TutorialController(dayCycleManager, customersManager);
            inittableServices.Add(tutorialController);

            await CreateVisual();
            await HandleInittableServices();

            saveLoadHelper.StartFollowSaves();
            playerInput.Activate(true);
        }
        public async UniTask CreateVisual()
        {
            IPlayerInput playerInput = diContainer.Resolve<IPlayerInput>();
            UnityEngine.Transform uiParent = sceneReferences.SceneCanvas.transform;
            new PizzaHolderBinder(pizzaHolder, sceneReferences, diContainer.Resolve<AllPizzaConfig>(), diContainer).Bind();
            new PizzaIngredientsHolderBinder(pizzaIngredientsHolder, sceneReferences, diContainer).Bind();
            new PizzaCreatorBinder(pizzaCreator, sceneReferences, diContainer).Bind();
            await new PizzeriaOrderHandlBinder(ordersHandler, pizzaCreator, assetsProvider, uiParent, deviceType, diContainer).Bind();
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
            if (pizzeriaComputer.GetAppByType(Computer.App.ComputerAppType.ManagmentApp) is ManagmentCompApp managmentApp)
            {
                new ManagmentCompAppBinder(managmentApp, sceneReferences, diContainer).Bind();
            }

            StatusPanelModel statusPanelModel = new(saveLoadHelper.PlayerWallet, dayCycleManager, pizzeriaManager);
            inittableServices.Add(statusPanelModel);
            await new StatusPanelBinder(statusPanelModel, assetsProvider, uiParent, diContainer).Bind();
            new PizzeriaManagerBinder(pizzeriaManager, sceneReferences, diContainer).Bind();

            await new PizzeriaDeliveryBinder(pizzeriaDelivery, deviceType, assetsProvider, uiParent, diContainer).Bind();

            new BoxesCarrierBinder(boxesCarrier, diContainer).Bind();

            await new TutorialVisualBinder(assetsProvider, tutorialController, uiParent, deviceType, diContainer).Bind();

            EndDayStatsModel endDayStatsModel = new(dayCycleManager, saveLoadHelper, saveLoadHelper.PlayerWallet,
                diContainer.Resolve<CustomersManager>(), playerInput);
            prewarmableServices.Add(endDayStatsModel);
            inittableServices.Add(endDayStatsModel);
            await new EndDayStatsVisualBinder(endDayStatsModel, assetsProvider, uiParent, diContainer).Bind();

            NewLevelPopUpModel newLevelPopUpModel = new(pizzeriaManager, playerInput, diContainer.Resolve<LevelsConfig>());
            inittableServices.Add(newLevelPopUpModel);
            await new NewLevelPopUpBinder(newLevelPopUpModel, assetsProvider, uiParent, diContainer).Bind();
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
            foreach (IPrewarmable prewarmableService in prewarmableServices)
            {
                await prewarmableService.Prewarm();
            }
            foreach (IInittable inittableService in inittableServices.OrderBy(inittable => inittable.InitPriority))
            {
                inittableService.Init();
                if(inittableService is ISceneDisposable disposable)
                {
                    diContainer.Bind<ISceneDisposable>().FromInstance(disposable);
                }
            }
            
            inittableServices.Clear();
        }
    }
}
