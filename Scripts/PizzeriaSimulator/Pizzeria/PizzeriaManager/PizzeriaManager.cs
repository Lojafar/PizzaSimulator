using Game.PizzeriaSimulator.Customers;
using Game.PizzeriaSimulator.Customers.Manager;
using Game.PizzeriaSimulator.DayCycle.Manager;
using Game.PizzeriaSimulator.Interactions;
using Game.PizzeriaSimulator.Interactions.Interactor;
using Game.PizzeriaSimulator.Levels.Config;
using Game.Root.ServicesInterfaces;
using R3;
using UnityEngine;
using Zenject;
namespace Game.PizzeriaSimulator.Pizzeria.Manager
{
    public class PizzeriaManager : IInittable, ISceneDisposable
    {
        public int InitPriority => 9;
        public int FirstLevel => levelsConfig.FirstLevel;
        public int MaxLevel => levelsConfig.MaxLevel;
        public bool LastOpenImmediately { get; private set; } = true;
        readonly ReactiveProperty<bool> opened;
        readonly ReactiveProperty<int> currentLevel;
        readonly ReactiveProperty<float> levelProgress;
        public ReadOnlyReactiveProperty<bool> Opened => opened;
        public ReadOnlyReactiveProperty<int> CurrentLevel => currentLevel;
        public ReadOnlyReactiveProperty<float> CurrentLevelProgress => levelProgress;
        readonly PizzeriaManagerData pizzeriaManagerData;
        readonly LevelsConfig levelsConfig;
        readonly DiContainer diContainer;
        CustomersManager customersManager;
        Interactor interactor;
        DayCycleManager dayCycleManager;
        int xpForNewLvl = 1;
        const int xpPerOrder = 1;
        public PizzeriaManager(PizzeriaManagerData _pizzeriaManagerData, LevelsConfig _levelsConfig, DiContainer _diContainer) 
        {
            levelsConfig = _levelsConfig;
            diContainer = _diContainer;
            pizzeriaManagerData = _pizzeriaManagerData ?? new PizzeriaManagerData(FirstLevel, 0, false, true);
            opened = new ReactiveProperty<bool>(pizzeriaManagerData.Opened);
            currentLevel = new ReactiveProperty<int>(pizzeriaManagerData.CurrentLevel);
            levelProgress = new ReactiveProperty<float>(-1);
        }
        public PizzeriaManagerData GetManagerData()
        {
            pizzeriaManagerData.Opened = opened.CurrentValue;
            pizzeriaManagerData.CurrentLevel = currentLevel.CurrentValue;
            return pizzeriaManagerData.Clone();
        }
        public void Init()
        {
            UpdateXpForNewLvl();
            LastOpenImmediately = true;
            levelProgress.Value = (float)pizzeriaManagerData.CurrentLvlXP / xpForNewLvl;
            interactor = diContainer.Resolve<Interactor>();
            customersManager = diContainer.Resolve<CustomersManager>();
            dayCycleManager = diContainer.Resolve<DayCycleManager>();
            customersManager.OnCustomerTakedOrder += OnCustomerTakedOrder;
            interactor.OnInteract += HandleInteractor;
            dayCycleManager.OnDayStarted += HandleDayStart;
            if (!pizzeriaManagerData.FirstOpenOfDay)
            {
                dayCycleManager.ResumeDay();
            }
        }
        public void Dispose()
        {
            customersManager.OnCustomerTakedOrder -= OnCustomerTakedOrder;
            interactor.OnInteract -= HandleInteractor;
            dayCycleManager.OnDayStarted -= HandleDayStart;
        }
        void UpdateXpForNewLvl()
        {
            if (currentLevel.CurrentValue > MaxLevel || currentLevel.CurrentValue < 0) return;
            xpForNewLvl = Mathf.FloorToInt(levelsConfig.XPForLevelCurve.Evaluate(currentLevel.CurrentValue));
        }
        void OnCustomerTakedOrder(Customer customer, int orderId)
        {
            pizzeriaManagerData.CurrentLvlXP += xpPerOrder;
            if (pizzeriaManagerData.CurrentLvlXP >= xpForNewLvl) 
            {
                pizzeriaManagerData.CurrentLvlXP = 0;
                currentLevel.Value++;
                UpdateXpForNewLvl();
            }
            levelProgress.Value = (float)pizzeriaManagerData.CurrentLvlXP / xpForNewLvl;
        }
        void HandleInteractor(InteractableType interactableType)
        {
            if(interactableType == InteractableType.OpenClosedSign)
            {
                if (opened.CurrentValue) Close();
                else Open();
            }
        }
        void HandleDayStart()
        {
            pizzeriaManagerData.FirstOpenOfDay = true;
            LastOpenImmediately = true;
            if (opened.CurrentValue == false) 
            {
                opened.OnNext(false);
            }
            else
            {
                opened.Value = false;
            }
        }
        public void Open()
        {
            if (opened.CurrentValue) return;
            LastOpenImmediately = false;
            if (pizzeriaManagerData.FirstOpenOfDay)
            {
                dayCycleManager.ResumeDay();
            }
            pizzeriaManagerData.FirstOpenOfDay = false;
            opened.Value = true;
        }
        public void Close()
        {
            if (!opened.CurrentValue) return;
            LastOpenImmediately = false;
            opened.Value = false;
        }
    }
}
