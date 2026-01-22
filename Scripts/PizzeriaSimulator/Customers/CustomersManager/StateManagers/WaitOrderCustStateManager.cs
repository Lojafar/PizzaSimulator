using Game.PizzeriaSimulator.OrdersHandle;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PizzeriaSimulator.Customers.Manager.StateManager
{
    class WaitOrderCustStateManager : ICustomerStateManager, IDisposable
    {
        readonly CustomersManager customersManager;
        readonly PizzeriaOrdersHandler ordersHandler;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly List<Customer> customersWaitesOrder;
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
        public void ForceCustomer(Customer customer)
        {
            customer.CustomerAI.SetState(CustomerState.WaitesOrder);
            Transform wayPoint = sceneReferences.CustomerWaitOrderField.GetWayPoint();
            customer.transform.SetPositionAndRotation(wayPoint.position, wayPoint.rotation);
            customer.CustomerAI.SetTargetPoint(wayPoint);
            customersWaitesOrder.Add(customer);
        }
    }
}
