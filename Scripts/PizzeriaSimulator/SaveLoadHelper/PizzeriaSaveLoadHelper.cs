using Game.Root.SaveLoad;
using Game.Root.ServicesInterfaces;
using Game.Root.Utils;
using Game.PizzeriaSimulator.Customers.Manager;
using Game.PizzeriaSimulator.Customers;
using Game.PizzeriaSimulator.PizzaCreation;
using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold;
using Game.PizzeriaSimulator.PizzaHold;
using Game.PizzeriaSimulator.Wallet;
using Game.PizzeriaSimulator.Pizzeria.Managment;
using Game.PizzeriaSimulator.DayCycle.Manager;
using R3;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Game.Root.AssetsManagment;
using Game.PizzeriaSimulator.SaveLoadHelp.Config;
using Game.PizzeriaSimulator.Pizzeria.Furniture.Placement.Manager;
namespace Game.PizzeriaSimulator.SaveLoadHelp
{
    public class DebounceSaving : IDisposable
    {
        readonly Action saveDelegate;
        float remainedTime;
        public DebounceSaving(Action _saveDelegate, float _remainedTime)
        {
            saveDelegate = _saveDelegate;
            remainedTime = _remainedTime;
            Ticks.Instance.OnTick += OnUpdate;
        }
        void OnUpdate()
        {
            remainedTime -= Time.deltaTime;
            if(remainedTime < 0)
            {
                Ticks.Instance.OnTick -= OnUpdate;
                saveDelegate?.Invoke();
            }
        }
        public void Dispose() 
        {
            Ticks.Instance.OnTick -= OnUpdate;
        }
    }
    public class PizzeriaSaveLoadHelper : ISceneDisposable
    {
        readonly ISaverLoader saverLoader;
        readonly IAssetsProvider assetsProvider;
        readonly DiContainer diContainer;
        readonly CompositeDisposable disposables;
        readonly Dictionary<byte, DebounceSaving> debounceSavings;
        PizzeriaInitSavesConfig initSavesConfig;
        public PlayerWallet PlayerWallet { get; private set; }
        PizzaIngredientsHolder ingredientsHolder;
        PizzaCreator pizzaCreator;
        PizzaHolder pizzaHolder;
        CustomersManager customersManager;
        PizzeriaManager pizzeriaManager;
        DayCycleManager dayCycleManager;
        PizzeriaFurnitureManager furnitureManager;
        bool isFollowingSaves;
        const byte pizzaHoldCode = 0;
        const byte customersManagerCode = 1;
        const byte ingredientsContCode = 2;
        const byte pizzeriaManagerCode = 3;
        public PizzeriaSaveLoadHelper(ISaverLoader _saverLoader, IAssetsProvider _assetsProvider, DiContainer _diContainer)
        {
            saverLoader = _saverLoader;
            assetsProvider = _assetsProvider;
            diContainer = _diContainer;
            disposables = new CompositeDisposable();
            debounceSavings = new Dictionary<byte, DebounceSaving>();
        }
        public async UniTask Prepare()
        {
            initSavesConfig = (await assetsProvider.LoadAsset<PizzeriaInitSavesConfigSO>(AssetsKeys.PizzeriaInitSavesConfig)).InitSavesConfig;
        }
        public async UniTask LoadAndBindSaves()
        {
            await LoadWallet();
            diContainer.Bind<PlayerWallet>().FromInstance(PlayerWallet).AsSingle();
        }
        async UniTask LoadWallet()
        {
            PlayerWalletData playerWalletData = await LoadOrTryGetInitData<PlayerWalletData>() ?? new PlayerWalletData();
            PlayerWallet = new PlayerWallet(playerWalletData);
        }
        public async UniTask<T> LoadData<T>(string key = null)
        {
            return await saverLoader.LoadData<T>(key);
        }
        public async UniTask<T> LoadOrTryGetInitData<T>(string key = null) where T : class
        {
            return ((await saverLoader.LoadData<T>(key)) ?? initSavesConfig.GetInitSaving<T>());
        }
        public async UniTask SaveData<T>(T data, string key = null)
        {
            await saverLoader.SaveData(data, key);
        }
        public async UniTask ClearData<T>(string key = null)
        {
            await saverLoader.ClearData<T>(key);
        }
        public void StartFollowSaves()
        {
            if (isFollowingSaves) return;
            isFollowingSaves = true;
            PlayerWallet.Money.Skip(1).Subscribe(_ => OnPlayerWalletChanged()).AddTo(disposables);
            PlayerWallet.Gems.Skip(1).Subscribe(_ => OnPlayerWalletChanged()).AddTo(disposables);
            ingredientsHolder = diContainer.TryResolve<PizzaIngredientsHolder>();
            if (ingredientsHolder != null)
            {
                ingredientsHolder.OnIngredientAdded += OnIngredientAdded;
            }
            pizzaCreator = diContainer.TryResolve<PizzaCreator>();
            if (pizzaCreator != null)
            {
                pizzaCreator.OnPizzaBake += OnPizzaBake;
                pizzaCreator.OnPizzaBaked += SavePizzaCreator;
            }
            pizzaHolder = diContainer.TryResolve<PizzaHolder>();
            if (pizzaHolder != null)
            {
                pizzaHolder.OnPizzaAdded += OnPizzaAddedInHold;
                pizzaHolder.OnPizzaReserved += OnPizzaHoldChanged;
                pizzaHolder.OnPizzaRemoved += OnPizzaHoldChanged;
            }
            customersManager = diContainer.TryResolve<CustomersManager>();
            if (customersManager != null)
            {
                customersManager.OnNewCustomer += OnSaveCustomersManager;
                customersManager.OnCustomerMadeOrder += OnCustomerChanged;
                customersManager.OnCustomerTakedOrder += OnCustomerChanged;
            }
            pizzeriaManager = diContainer.TryResolve<PizzeriaManager>();
            if (pizzeriaManager != null)
            {
                pizzeriaManager.CurrentLevel.Skip(1).Subscribe(_ => SavePizzeriaManger()).AddTo(disposables);
                pizzeriaManager.CurrentLevelProgress.Skip(1).Subscribe(_ => SavePizzeriaManger()).AddTo(disposables);
                pizzeriaManager.Opened.Skip(1).Subscribe(OnPizzeriaOpenOrClose).AddTo(disposables);
                pizzeriaManager.OnExpansionUnlocked += OnPizzeriaExpanUnlk;
            }
            dayCycleManager = diContainer.TryResolve<DayCycleManager>();
            if (dayCycleManager != null)
            {
                dayCycleManager.Minutes.Subscribe(OnDayMinutesChanged).AddTo(disposables);
                dayCycleManager.OnDayStarted += OnDayStarted;
                dayCycleManager.OnDayEnded += SaveDayCycleManager;
                dayCycleManager.IsDayPaused.Skip(1).Subscribe(OnDayPaused).AddTo(disposables);
            }
            furnitureManager = diContainer.TryResolve<PizzeriaFurnitureManager>();
            if(furnitureManager != null)
            {
                furnitureManager.OnNewFurniturePlaced += OnFurniturePlaced;
            }
        }
        public void Dispose()
        {
            if (!isFollowingSaves) return;
            isFollowingSaves = false;
            disposables.Dispose();
            if (ingredientsHolder != null)
            {
                ingredientsHolder.OnIngredientAdded -= OnIngredientAdded;
            }
            if (pizzaCreator != null)
            {
                pizzaCreator.OnPizzaBake -= OnPizzaBake;
                pizzaCreator.OnPizzaBaked -= SavePizzaCreator;
            }
            if (pizzaHolder != null)
            {
                pizzaHolder.OnPizzaAdded -= OnPizzaAddedInHold;
                pizzaHolder.OnPizzaReserved -= OnPizzaHoldChanged;
                pizzaHolder.OnPizzaRemoved -= OnPizzaHoldChanged;
            }
            if (customersManager != null)
            {
                customersManager.OnNewCustomer -= OnSaveCustomersManager;
                customersManager.OnCustomerMadeOrder -= OnCustomerChanged;
                customersManager.OnCustomerTakedOrder -= OnCustomerChanged;
            }
            if (pizzeriaManager != null) 
            {
                pizzeriaManager.OnExpansionUnlocked -= OnPizzeriaExpanUnlk;
            }
            if (dayCycleManager != null)
            {
                dayCycleManager.OnDayStarted -= OnDayStarted;
                dayCycleManager.OnDayEnded -= SaveDayCycleManager;
            }
            if (furnitureManager != null)
            {
                furnitureManager.OnNewFurniturePlaced -= OnFurniturePlaced;
            }
            foreach (DebounceSaving debounceSaving in debounceSavings.Values)
            {
                debounceSaving.Dispose();
            }
        }
        void OnPlayerWalletChanged()
        {
            saverLoader.SaveData(PlayerWallet.GetOriginData()).Forget();
        }
        void OnIngredientAdded(PizzaIngredientType type, bool seldAdd)
        {
            if (debounceSavings.TryGetValue(ingredientsContCode, out DebounceSaving debounceSaving))
            {
                debounceSaving.Dispose();
            }
            debounceSavings[ingredientsContCode] = new DebounceSaving(SaveIngredients, 1f);
        }
        void SaveIngredients()
        {
            debounceSavings[ingredientsContCode].Dispose();
            debounceSavings.Remove(ingredientsContCode);
            saverLoader.SaveData(ingredientsHolder.IngredientsHolderData).Forget();
        }
        void OnPizzaBake(int pizzaId)
        {
            saverLoader.SaveData(ingredientsHolder.IngredientsHolderData).Forget();
            SavePizzaCreator(pizzaId);
        }
        void SavePizzaCreator(int pizzaID)
        {
            saverLoader.SaveData(pizzaCreator.PizzaCreatorData).Forget();
        }
        void OnPizzaAddedInHold(int pizzaID)
        {
            if (pizzaCreator.PizzasInBakeCount < 1)
            {
                saverLoader.SaveData(pizzaCreator.PizzaCreatorData).Forget();
            }
            OnPizzaHoldChanged(pizzaID);
        }
        void OnPizzaHoldChanged(int pizzaId)
        {
            if (debounceSavings.TryGetValue(pizzaHoldCode, out DebounceSaving debounceSaving))
            {
                debounceSaving.Dispose();
            }
            debounceSavings[pizzaHoldCode] = new DebounceSaving(SavePizzaHold, 0.5f);
        }
        void SavePizzaHold()
        {
            debounceSavings[pizzaHoldCode].Dispose();
            debounceSavings.Remove(pizzaHoldCode);
            saverLoader.SaveData(pizzaHolder.PizzaHolderData).Forget();
        }
        void OnCustomerChanged(Customer customer, int order)
        {
            OnSaveCustomersManager(customer);
        }
        void OnSaveCustomersManager(Customer customers)
        {
            if (debounceSavings.TryGetValue(customersManagerCode, out DebounceSaving debounceSaving))
            {
                debounceSaving.Dispose();
            }
            debounceSavings[customersManagerCode] = new DebounceSaving(SaveCustomersManager, 1f);
        }
        void SaveCustomersManager()
        {
            debounceSavings[customersManagerCode].Dispose();
            debounceSavings.Remove(customersManagerCode);
            saverLoader.SaveData(customersManager.ManagerData).Forget();
        }
        
        void OnPizzeriaOpenOrClose(bool open)
        {
            if (debounceSavings.TryGetValue(pizzeriaManagerCode, out DebounceSaving debounceSaving))
            {
                debounceSaving.Dispose();
            }
            debounceSavings[pizzeriaManagerCode] = new DebounceSaving(OnPizzeriaMangerDebounced, 0.4f);
        }
        void OnPizzeriaMangerDebounced()
        {
            if (debounceSavings.TryGetValue(pizzeriaManagerCode, out DebounceSaving debounceSaving))
            {
                debounceSaving.Dispose();
                debounceSavings.Remove(pizzeriaManagerCode);
            }
            SavePizzeriaManger();
        }
        void OnPizzeriaExpanUnlk(int expansionID)
        {
            SavePizzeriaManger();
        }
        void SavePizzeriaManger()
        {
            saverLoader.SaveData(pizzeriaManager.GetManagerData()).Forget();
        }
        const int saveDayCycleDelay = 15;
        const int minutesInHour = 60;
        int lastSaveDayCycleMinutes;
        void OnDayMinutesChanged(int newMinutes)
        {
            if (dayCycleManager.Hours.CurrentValue * minutesInHour + newMinutes - lastSaveDayCycleMinutes < saveDayCycleDelay) return;
            lastSaveDayCycleMinutes = dayCycleManager.Hours.CurrentValue * minutesInHour + newMinutes;
            SaveDayCycleManager();
        }
        void OnDayStarted()
        {
            lastSaveDayCycleMinutes = 0;
            SaveDayCycleManager();
        }
        void OnDayPaused(bool pause)
        {
            SaveDayCycleManager();
        }
        void SaveDayCycleManager()
        {
            saverLoader.SaveData(dayCycleManager.GetManagerData()).Forget();
        }
        void OnFurniturePlaced(int id)
        {
            saverLoader.SaveData(furnitureManager.ManagerData).Forget();
        }
    }
}
