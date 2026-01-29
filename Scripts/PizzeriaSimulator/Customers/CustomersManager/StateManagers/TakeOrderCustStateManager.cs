using Game.PizzeriaSimulator.OrdersHandle;
using System;
using System.Collections.Generic;

namespace Game.PizzeriaSimulator.Customers.Manager.StateManager
{
    class TakeOrderCustStateManager : ICustomerStateManager
    {
        public event Action<Customer, int> OnCustomerTakedOrder;
        readonly CustomersManager customersManager;
        readonly PizzeriaOrdersHandler ordersHandler;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly List<Customer> customers;
        public TakeOrderCustStateManager(CustomersManager _customersManager, PizzeriaOrdersHandler _ordersHandler, PizzeriaSceneReferences _sceneReferences)
        {
            customersManager = _customersManager;
            ordersHandler = _ordersHandler;
            sceneReferences = _sceneReferences;
            customers = new List<Customer>();
        }
        public void ForceCustomer(Customer customer)
        {
            customer.CustomerAI.SetState(CustomerState.TakesOrder);
            ordersHandler.TakeOrder(customer.OrderId);
            OnCustomerTakedOrder?.Invoke(customer, customer.OrderId);
            customersManager.SwitchCustomerStateManager(customer, CustomerState.Leaves);
        }
        public void HandleCustomerOfState(Customer customer)
        {
            customer.CustomerAI.OnTargetPointReached += OnCustomerReachedTakePoint;
            customer.CustomerAI.SetTargetPoint(sceneReferences.CustomersTakeOrderPoint);
            customers.Add(customer);
        }
        void OnCustomerReachedTakePoint(int customerID)
        {
            for (int i = 0; i < customers.Count; i++) 
            {
                if (customers[i].Id == customerID)
                {
                    Customer customer = customers[i];
                    customers.RemoveAt(i);
                    customer.CustomerAI.OnTargetPointReached -= OnCustomerReachedTakePoint;
                    customer.CustomerAI.SetState(CustomerState.TakesOrder);
                    ordersHandler.TakeOrder(customer.OrderId);
                    OnCustomerTakedOrder?.Invoke(customer, customer.OrderId);
                    customersManager.SwitchCustomerStateManager(customer, CustomerState.Leaves);
                    return;
                }
            }
        }
        public void RemoveCustomer(Customer customer)
        {
            for (int i = 0; i < customers.Count; i++)
            {
                if (customers[i].Id == customer.Id)
                {
                    customers.RemoveAt(i);
                    customer.CustomerAI.OnTargetPointReached -= OnCustomerReachedTakePoint;
                }
            }
        }
    }
}
