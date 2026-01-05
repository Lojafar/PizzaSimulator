using Game.PizzeriaSimulator.OrdersHandle;
using System;
using System.Collections.Generic;

namespace Game.PizzeriaSimulator.Customers.Manager.StateManager
{
    class WaitOrderCustStateManager : ICustomerStateManager, IDisposable
    {
        readonly CustomersManager customersManager;
        readonly PizzeriaOrdersHandler ordersHandler;
        readonly PizzeriaSceneReferences sceneReferences;
        List<Customer> customersWaitesOrder;
        public WaitOrderCustStateManager(CustomersManager _customersManager, PizzeriaOrdersHandler _ordersHandler, PizzeriaSceneReferences _sceneReferences)
        {
            customersManager = _customersManager;
            ordersHandler = _ordersHandler;
            sceneReferences = _sceneReferences;
            customersWaitesOrder = new List<Customer>();
            ordersHandler.OnPizzaOrderCompleted += HandleCompletedOrder;
        }
        public void Dispose()
        {
            ordersHandler.OnPizzaOrderCompleted -= HandleCompletedOrder;
        }
        void HandleCompletedOrder(int orderID)
        {
            foreach (Customer customer in customersWaitesOrder)
            {
                if (customer.OrderId == orderID)
                {
                    customer.CustomerAI.SetTargetPoint(sceneReferences.CustomersTakeOrderPoint);
                    customersWaitesOrder.Remove(customer);
                    customersManager.SwitchCustomerStateManager(customer, CustomerState.TakesOrder);
                    return;
                }
            }
        }
        public void HandleCustomerOfState(Customer customer)
        {
            customer.CustomerAI.SetState(CustomerState.WaitesOrder);
            customer.CustomerAI.SetTargetPoint(sceneReferences.CustomerWaitOrderField.GetWayPoint());
            customersWaitesOrder.Add(customer);
        }
    }
}
