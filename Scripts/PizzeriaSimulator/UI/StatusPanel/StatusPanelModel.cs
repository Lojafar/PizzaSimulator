using Game.PizzeriaSimulator.Currency;
using Game.PizzeriaSimulator.DayCycle.Manager;
using Game.PizzeriaSimulator.Pizzeria.Manager;
using Game.PizzeriaSimulator.Wallet;
using Game.Root.ServicesInterfaces;
using R3;
using System;
namespace Game.PizzeriaSimulator.UI.StatusPanel
{
    class StatusPanelModel : IInittable, ISceneDisposable
    {
        public int InitPriority => 10;
        public Action<MoneyQuantity> OnMoneyChanged;
        public Action<int> OnHoursTimeChanged;
        public Action<int> OnMinutesTimeChanged;
        public Action<int> OnNewPizzeriaLevel;
        public Action<float> OnPizzeriaLvlProgress;
        readonly PlayerWallet playerWallet;
        readonly DayCycleManager dayCycleManager;
        readonly PizzeriaManager pizzeriaManager;
        readonly CompositeDisposable disposables;
        public StatusPanelModel(PlayerWallet _playerWallet, DayCycleManager _dayCycleManager, PizzeriaManager _pizzeriaManager) 
        {
            playerWallet = _playerWallet;
            dayCycleManager = _dayCycleManager;
            pizzeriaManager = _pizzeriaManager;
            disposables = new CompositeDisposable();
        }
        public void Init()
        {
            playerWallet.Money.Subscribe(HandleMoneyInWallet).AddTo(disposables);
            dayCycleManager.Hours.Subscribe(HandleNewHour).AddTo(disposables);
            dayCycleManager.Minutes.Subscribe(HandleNewMinutes).AddTo(disposables);
            pizzeriaManager.CurrentLevel.Subscribe(HandlePizzeriaLevel).AddTo(disposables);
            pizzeriaManager.CurrentLevelProgress.Subscribe(HandlePizzeriaLvlProgress).AddTo(disposables);
        }
        public void Dispose() 
        {
            disposables.Dispose();
        }
        void HandleMoneyInWallet(MoneyQuantity moneyQuantity)
        {
            OnMoneyChanged?.Invoke(moneyQuantity);
        }
        void HandleNewHour(int hour)
        {
            OnHoursTimeChanged?.Invoke(hour);
        }
        void HandleNewMinutes(int minutes)
        {
            OnMinutesTimeChanged?.Invoke(minutes);
        }
        void HandlePizzeriaLevel(int level) 
        {
            OnNewPizzeriaLevel?.Invoke(level);
        }
        void HandlePizzeriaLvlProgress(float progress)
        {
            OnPizzeriaLvlProgress?.Invoke(progress);
        }
    }
}
