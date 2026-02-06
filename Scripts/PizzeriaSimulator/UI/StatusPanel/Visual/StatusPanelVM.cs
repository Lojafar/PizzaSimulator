using Game.PizzeriaSimulator.Currency;
using Game.Root.ServicesInterfaces;
using System;

namespace Game.PizzeriaSimulator.UI.StatusPanel.Visual
{
    class StatusPanelVM : ISceneDisposable
    {
        public event Action<string> UpdateMoneyText;
        public event Action<string> UpdateGemsText;
        public event Action<string> UpdateTimeText;
        public event Action<string> UpdateLevelText;
        public event Action<float> UpdateLvlProgress;
        readonly StatusPanelModel statusPanelModel;
        int currentHour;
        const string moneyTextPattern = "${0}.{1:D2}";
        const string timePattern = "{0:D2}:{1:D2}";
        const string lvlPattern = "Level {0}";
        public StatusPanelVM(StatusPanelModel _statusPanelModel)
        {
            statusPanelModel = _statusPanelModel;
        }
        public void Init()
        {
            statusPanelModel.OnMoneyChanged += HandleNewMoney;
            statusPanelModel.OnGemsChanged += HandleNewGems;
            statusPanelModel.OnHoursTimeChanged += HandleHours;
            statusPanelModel.OnMinutesTimeChanged += HandleMinutes;
            statusPanelModel.OnNewPizzeriaLevel += HandlePizzeriaLvl;
            statusPanelModel.OnPizzeriaLvlProgress += HandleLvlProgress;
        }
        public void Dispose()
        {
            statusPanelModel.OnMoneyChanged -= HandleNewMoney;
            statusPanelModel.OnGemsChanged -= HandleNewGems;
            statusPanelModel.OnHoursTimeChanged -= HandleHours;
            statusPanelModel.OnMinutesTimeChanged -= HandleMinutes;
            statusPanelModel.OnNewPizzeriaLevel -= HandlePizzeriaLvl;
            statusPanelModel.OnPizzeriaLvlProgress -= HandleLvlProgress;
        }
        void HandleNewMoney(MoneyQuantity moneyQuantity)
        {
            UpdateMoneyText?.Invoke(string.Format(moneyTextPattern, moneyQuantity.Dollars, moneyQuantity.Cents));
        }
        void HandleNewGems(int gemsAmount)
        {
            UpdateGemsText?.Invoke(gemsAmount.ToString());
        }
        void HandleHours(int hours)
        {
            currentHour = hours;
        }
        void HandleMinutes(int minutes)
        {
            UpdateTimeText?.Invoke(string.Format(timePattern, currentHour, minutes));
        }
        void HandlePizzeriaLvl(int level)
        {
            UpdateLevelText?.Invoke(string.Format(lvlPattern, level));
        }
        void HandleLvlProgress(float progress)
        {
            UpdateLvlProgress?.Invoke(progress);
        }
    }
}
