using Game.PizzeriaSimulator.Currency;
using Game.Root.ServicesInterfaces;
using System;

namespace Game.PizzeriaSimulator.UI.EndDayStatsPanel.Visual
{
    class EndDayStatsVM : ISceneDisposable
    {
        public event Action<bool> OnOpen;
        public event Action<bool> OnClose;
        public event Action<string> UpdateCustomersText;
        public event Action<string> UpdateUnitsText;
        public event Action<string, bool> UpdatePurchasesText;
        public event Action<string, bool> UpdateSalesText;
        public event Action<string, bool> UpdateTipsText;
        public event Action<string, bool> UpdateTotalProfitText;
        readonly EndDayStatsModel endDayStatsModel;
        const string moneyPatternPositive = "+${0}.{1}";
        const string moneyPatternNegative = "-${0}.{1}";
        public EndDayStatsVM(EndDayStatsModel _endDayStatsModel) 
        {
            endDayStatsModel = _endDayStatsModel;
        }
        public void Init()
        {
            endDayStatsModel.OnOpen += HandleOpen;
            endDayStatsModel.OnClose += HandleClose;
            endDayStatsModel.NewTotalCustomers += HandleTotalCustomers;
            endDayStatsModel.NewUnitsSoldAmount += HandleUnitsSold;
            endDayStatsModel.NewTotalPurchases += HandleTotalPurchases;
            endDayStatsModel.NewTotalSales += HandleTotalSales;
            endDayStatsModel.NewTotalTips += HandleTotalTips;
            endDayStatsModel.NewTotalProfit += HandleTotalProfit;
            OnClose?.Invoke(true);
        }
        public void Dispose()
        {
            endDayStatsModel.OnOpen -= HandleOpen;
            endDayStatsModel.OnClose -= HandleClose;
            endDayStatsModel.NewTotalCustomers -= HandleTotalCustomers;
            endDayStatsModel.NewUnitsSoldAmount -= HandleUnitsSold;
            endDayStatsModel.NewTotalPurchases -= HandleTotalPurchases;
            endDayStatsModel.NewTotalSales -= HandleTotalSales;
            endDayStatsModel.NewTotalTips -= HandleTotalTips;
            endDayStatsModel.NewTotalProfit -= HandleTotalProfit;
        }
        public void ConfirmInput()
        {
            endDayStatsModel.CloseInput();
        }
        void HandleOpen()
        {
            OnOpen?.Invoke(false);
        }
        void HandleClose()
        {
            OnClose?.Invoke(false);
        }
        void HandleTotalCustomers(int totalCustomers) 
        {
            UpdateCustomersText?.Invoke(totalCustomers.ToString());
        }
        void HandleUnitsSold(int unitsSold)
        {
            UpdateUnitsText?.Invoke(unitsSold.ToString());
        }
        void HandleTotalPurchases(MoneyQuantity totalPurchases)
        {
            UpdatePurchasesText?.Invoke(string.Format(moneyPatternNegative, totalPurchases.Dollars, totalPurchases.Cents), totalPurchases == MoneyQuantity.Zero);
        }
        void HandleTotalSales(MoneyQuantity totalSales)
        {
            UpdateSalesText?.Invoke(string.Format(moneyPatternPositive, totalSales.Dollars, totalSales.Cents), true);
        }
        void HandleTotalTips(MoneyQuantity totalTips)
        {
            UpdateTipsText?.Invoke(string.Format(moneyPatternPositive, totalTips.Dollars, totalTips.Cents), true);
        }
        void HandleTotalProfit(MoneyQuantity totalProfit)
        {
            if (totalProfit < MoneyQuantity.Zero)
            {
                totalProfit *= -1;
                UpdateTotalProfitText(string.Format(moneyPatternNegative, totalProfit.Dollars, totalProfit.Cents), false);
            }
            else
            {
                UpdateTotalProfitText(string.Format(moneyPatternPositive, totalProfit.Dollars, totalProfit.Cents), true);
            }
        }
    }
}
