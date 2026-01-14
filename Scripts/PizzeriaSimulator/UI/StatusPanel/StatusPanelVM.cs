using Game.PizzeriaSimulator.Currency;
using Game.Root.ServicesInterfaces;
using System;

namespace Game.PizzeriaSimulator.UI.StatusPanel
{
    class StatusPanelVM : ISceneDisposable
    {
        public event Action<string> UpdateMoneyText;
        readonly StatusPanelModel statusPanelModel;
        const string moneyTextPattern = "${0}.{1:D2}";
        public StatusPanelVM(StatusPanelModel _statusPanelModel)
        {
            statusPanelModel = _statusPanelModel;
        }
        public void Init()
        {
            statusPanelModel.OnMoneyChanged += HandleNewMoney;
        }
        public void Dispose()
        {
            statusPanelModel.OnMoneyChanged -= HandleNewMoney;
        }
        void HandleNewMoney(MoneyQuantity moneyQuantity)
        {
            UpdateMoneyText?.Invoke(string.Format(moneyTextPattern, moneyQuantity.Dollars, moneyQuantity.Cents));
        }
    }
}
