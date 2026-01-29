using Game.Root.ServicesInterfaces;
using Game.PizzeriaSimulator.Currency;
using Game.PizzeriaSimulator.Customers;
using Game.PizzeriaSimulator.Customers.Manager;
using Game.PizzeriaSimulator.SaveLoadHelp;
using Game.PizzeriaSimulator.Wallet;
using Game.PizzeriaSimulator.DayCycle.Manager;
using Game.PizzeriaSimulator.Player.Input;
using Cysharp.Threading.Tasks;
using R3;
using System;

namespace Game.PizzeriaSimulator.UI.EndDayStatsPanel
{
    class EndDayStatsModel : IPrewarmable, IInittable, ISceneDisposable
    {
        public int InitPriority => 10;
        public event Action OnOpen;
        public event Action OnClose;
        public event Action<int> NewTotalCustomers;
        public event Action<int> NewUnitsSoldAmount;
        public event Action<MoneyQuantity> NewTotalPurchases;
        public event Action<MoneyQuantity> NewTotalSales;
        public event Action<MoneyQuantity> NewTotalTips;
        public event Action<MoneyQuantity> NewTotalProfit;
        readonly DayCycleManager dayCycleManager;
        readonly PizzeriaSaveLoadHelper saveLoadHelper;
        readonly PlayerWallet playerWallet;
        readonly CustomersManager customersManager;
        readonly IPlayerInput playerInput;
        EndDayStatsModelData modelData;
        IDisposable moneySubscription;
        MoneyQuantity lastCheckedMoney;
        public EndDayStatsModel(DayCycleManager _dayCycleManager, PizzeriaSaveLoadHelper _saveLoadHelper, PlayerWallet _playerWallet, 
            CustomersManager _customersManager, IPlayerInput _playerInput)
        {
            dayCycleManager = _dayCycleManager;
            saveLoadHelper = _saveLoadHelper;
            playerWallet = _playerWallet;
            customersManager = _customersManager;
            playerInput = _playerInput;
        }
        public async UniTask Prewarm()
        {
            modelData = await saveLoadHelper.LoadData<EndDayStatsModelData>();
            modelData ??= new EndDayStatsModelData();
        }
        public void Init()
        {
            lastCheckedMoney = playerWallet.Money.CurrentValue;
            dayCycleManager.OnDayStarted += OnDayStarted;
            moneySubscription = playerWallet.Money.Skip(1).Subscribe(OnMoneyInWalletChanged);
            customersManager.OnNewCustomer += OnNewCustomer;
            customersManager.OnCustomerTakedOrder += OnCustomerTakedOrder;
        }
        public void Dispose()
        {
            moneySubscription?.Dispose();
            dayCycleManager.OnDayStarted -= OnDayStarted;
            customersManager.OnNewCustomer -= OnNewCustomer;
            customersManager.OnCustomerTakedOrder -= OnCustomerTakedOrder;
        }
        void OnDayStarted()
        {
            NewTotalCustomers?.Invoke(modelData.CustomersOfDay);
            NewUnitsSoldAmount?.Invoke(modelData.UnitsSold);
            NewTotalPurchases?.Invoke(modelData.TotalPurchases);
            NewTotalSales?.Invoke(modelData.TotalSales);
            NewTotalTips?.Invoke(modelData.TotalTips);
            NewTotalProfit?.Invoke(modelData.TotalTips + modelData.TotalSales - modelData.TotalPurchases);
            OnOpen?.Invoke();
            playerInput.Activate(false);

            modelData.CustomersOfDay = 0;
            modelData.UnitsSold = 0;
            modelData.TotalPurchases = MoneyQuantity.Zero;
            modelData.TotalSales = MoneyQuantity.Zero;
            modelData.TotalTips = MoneyQuantity.Zero;
            saveLoadHelper.SaveData(modelData).Forget();
        }
       
        void OnMoneyInWalletChanged(MoneyQuantity newMoneyQuantity)
        {
            MoneyQuantity moneyDelta = newMoneyQuantity - lastCheckedMoney;
            lastCheckedMoney = newMoneyQuantity;
            if (moneyDelta < MoneyQuantity.Zero)
            {
                modelData.TotalPurchases -= moneyDelta;
            }
            else
            {
                modelData.TotalSales += moneyDelta;
            }
            saveLoadHelper.SaveData(modelData).Forget();
        }
        void OnNewCustomer(Customer customer)
        {
            modelData.CustomersOfDay++; 
            saveLoadHelper.SaveData(modelData).Forget();
        }
        void OnCustomerTakedOrder(Customer customer, int orderID)
        {
            modelData.UnitsSold++;
            saveLoadHelper.SaveData(modelData).Forget();
        }
        public void CloseInput()
        {
            OnClose?.Invoke();
            playerInput.Activate(true);
        }

    }
}
