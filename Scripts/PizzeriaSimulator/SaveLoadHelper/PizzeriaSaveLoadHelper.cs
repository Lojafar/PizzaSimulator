using Game.Root.SaveLoad;
using Game.Root.ServicesInterfaces;
using Game.Root.Utils;
using Game.PizzeriaSimulator.Customers.Manager;
using Game.PizzeriaSimulator.Customers;
using Game.PizzeriaSimulator.PizzaCreation;
using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold;
using Game.PizzeriaSimulator.PizzaHold;
using Game.PizzeriaSimulator.Wallet;
using R3;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
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
        readonly DiContainer diContainer;
        readonly CompositeDisposable disposables;
        readonly Dictionary<byte, DebounceSaving> debounceSavings;
        public PlayerWallet PlayerWallet { get; private set; }
        PizzaIngredientsHolder ingredientsHolder;
        PizzaCreator pizzaCreator;
        PizzaHolder pizzaHolder;
        CustomersManager customersManager;
        bool isFollowingSaves;
        const byte PizzaHoldCode = 0;
        const byte CustomersManagerCode = 1;
        const byte IngredientsContCode = 2;
        public PizzeriaSaveLoadHelper(ISaverLoader _saverLoader, DiContainer _diContainer)
        {
            saverLoader = _saverLoader;
            diContainer = _diContainer;
            disposables = new CompositeDisposable();
            debounceSavings = new Dictionary<byte, DebounceSaving>();
        }
        public async UniTask LoadAndBindSaves()
        {
            await LoadWallet();
            diContainer.Bind<PlayerWallet>().FromInstance(PlayerWallet).AsSingle();
        }
        async UniTask LoadWallet()
        {
            PlayerWalletData playerWalletData = await saverLoader.LoadData<PlayerWalletData>() ?? new PlayerWalletData();
            PlayerWallet = new PlayerWallet(playerWalletData);
        }
        public void StartFollowSaves()
        {
            if (isFollowingSaves) return;
            isFollowingSaves = true;
            PlayerWallet.Money.Skip(1).Subscribe(_ => OnWalletMoneyChanged()).AddTo(disposables);
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
            foreach (DebounceSaving debounceSaving in debounceSavings.Values)
            {
                debounceSaving.Dispose();
            }
        }
        void OnWalletMoneyChanged()
        {
            saverLoader.SaveData(PlayerWallet.GetOriginData());
        }
        void OnIngredientAdded(PizzaIngredientType type, bool seldAdd)
        {
            if (debounceSavings.TryGetValue(IngredientsContCode, out DebounceSaving debounceSaving))
            {
                debounceSaving.Dispose();
            }
            debounceSavings[IngredientsContCode] = new DebounceSaving(SaveIngredients, 1f);
        }
        void SaveIngredients()
        {
            debounceSavings[IngredientsContCode].Dispose();
            debounceSavings.Remove(IngredientsContCode);
            saverLoader.SaveData(ingredientsHolder.IngredientsHolderData);
        }
        void OnPizzaBake(int pizzaId)
        {
            saverLoader.SaveData(ingredientsHolder.IngredientsHolderData);
            SavePizzaCreator(pizzaId);
        }
        void SavePizzaCreator(int pizzaID)
        {
            saverLoader.SaveData(pizzaCreator.PizzaCreatorData);
        }
        void OnPizzaAddedInHold(int pizzaID)
        {
           if (pizzaCreator.PizzasInBakeCount < 1)
           {
               saverLoader.SaveData(pizzaCreator.PizzaCreatorData);
           }
            OnPizzaHoldChanged(pizzaID);
        }
        void OnPizzaHoldChanged(int pizzaId)
        {
            if (debounceSavings.TryGetValue(PizzaHoldCode, out DebounceSaving debounceSaving))
            {
                debounceSaving.Dispose();
            }
            debounceSavings[PizzaHoldCode] = new DebounceSaving(SavePizzaHold, 0.5f);
        }
        void SavePizzaHold()
        {
            debounceSavings[PizzaHoldCode].Dispose();
            debounceSavings.Remove(PizzaHoldCode);
            saverLoader.SaveData(pizzaHolder.PizzaHolderData);
        }
        void OnCustomerChanged(Customer customer, int order)
        {
            OnSaveCustomersManager(customer);
        }
        void OnSaveCustomersManager(Customer customers)
        {
            if (debounceSavings.TryGetValue(CustomersManagerCode, out DebounceSaving debounceSaving))
            {
                debounceSaving.Dispose();
            }
            debounceSavings[CustomersManagerCode] = new DebounceSaving(SaveCustomersManager, 1f);
        }
        void SaveCustomersManager()
        {
            debounceSavings[CustomersManagerCode].Dispose();
            debounceSavings.Remove(CustomersManagerCode);
            saverLoader.SaveData(customersManager.ManagerData);
        }
        public async UniTask<T> LoadData<T>(string key = null)
        {
            return await saverLoader.LoadData<T>(key);
        }
        public async UniTask SaveData<T>(T data, string key = null)
        {
            await saverLoader.SaveData(data, key);
        }
        public async UniTask ClearData<T>(string key = null)
        {
            await saverLoader.ClearData<T>(key);
        }
    }
}
