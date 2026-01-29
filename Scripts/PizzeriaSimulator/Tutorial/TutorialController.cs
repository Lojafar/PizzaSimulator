using Game.PizzeriaSimulator.Customers;
using Game.PizzeriaSimulator.Customers.Manager;
using Game.PizzeriaSimulator.DayCycle.Manager;
using Game.Root.ServicesInterfaces;
using System;

namespace Game.PizzeriaSimulator.Tutorial
{
    class TutorialController : IInittable, ISceneDisposable
    {
        public int InitPriority => 12;
        readonly DayCycleManager dayCycleManager;
        readonly CustomersManager customersManager;
        public event Action OnEndDayTask;
        public event Action<int> OnNewTask;
        bool isEndDayTaskActive;
        public TutorialController(DayCycleManager _dayCycleManager, CustomersManager _customersManager)
        {
            dayCycleManager = _dayCycleManager;
            customersManager = _customersManager;
        }
        public void Init()
        {
            dayCycleManager.OnDayStarted += HandleDayStart;
            dayCycleManager.OnDayEnded += HandleDayEnd;
            customersManager.OnCustomerTakedOrder += HandleCustomerTakeOrder;

            OnNewTask?.Invoke(1);
        }
        public void Dispose()
        {
            dayCycleManager.OnDayStarted -= HandleDayStart;
            dayCycleManager.OnDayEnded -= HandleDayEnd;
            customersManager.OnCustomerTakedOrder -= HandleCustomerTakeOrder;
        }
        void HandleDayStart()
        {
            isEndDayTaskActive = false;
        }
        void HandleDayEnd()
        {
            if (IsEndDayTaskPossible())
            {
                OnEndDayTask?.Invoke();
            }
        }
        void HandleCustomerTakeOrder(Customer customer, int orderId)
        {
            if (IsEndDayTaskPossible())
            {
                OnEndDayTask?.Invoke();
            }
        }
        public void EndDayInput()
        {
            if (!IsEndDayTaskPossible()) return;
            isEndDayTaskActive = true;
            dayCycleManager.RestartDay();
            customersManager.DestroyAllCustomers();
            OnNewTask?.Invoke(1);
        }
        bool IsEndDayTaskPossible()
        {
            return !isEndDayTaskActive && dayCycleManager.IsDayEnded && customersManager.ActiveCustomersCount < 1;
        }
    }
}
