using Game.PizzeriaSimulator.Currency;
using System;

namespace Game.PizzeriaSimulator.UI.EndDayStatsPanel
{
    [Serializable]
    class EndDayStatsModelData
    {
        public int CustomersOfDay;
        public int UnitsSold;
        public MoneyQuantity TotalPurchases;
        public MoneyQuantity TotalSales;
        public MoneyQuantity TotalTips;
    }
}
