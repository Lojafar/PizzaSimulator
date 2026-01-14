using Game.PizzeriaSimulator.Currency;
using Game.PizzeriaSimulator.Wallet;
using Game.Root.ServicesInterfaces;
using R3;
using System;
namespace Game.PizzeriaSimulator.UI.StatusPanel
{
    class StatusPanelModel : IInittable, ISceneDisposable
    {
        public Action<MoneyQuantity> OnMoneyChanged;
        readonly PlayerWallet playerWallet;
        readonly CompositeDisposable disposables;
        public StatusPanelModel(PlayerWallet _playerWallet) 
        {
            playerWallet = _playerWallet;
            disposables = new CompositeDisposable();
        }
        public void Init()
        {
            playerWallet.Money.Subscribe(OnNewMoneyInWallet).AddTo(disposables);
        }
        public void Dispose() 
        {
            disposables.Dispose();
        }
        void OnNewMoneyInWallet(MoneyQuantity moneyQuantity)
        {
            OnMoneyChanged?.Invoke(moneyQuantity);
        }
    }
}
