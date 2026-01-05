using Game.PizzeriaSimulator.Customers.OrdersProcces;
using System;
using System.Collections.Generic;
namespace Game.PizzeriaSimulator.Customers.Manager.StateManager
{
    using Random = UnityEngine.Random;
    class LineCustomerStateManager : ICustomerStateManager
    {
        public event Action<Customer, int> OnCustomerStartOrder;
        public event Action<Customer, int> OnCustomerMadeOrder;
        readonly CustomersManager customersManager;
        readonly CustomersOrdersProccesor customersOrdersProccesor;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly List<Customer> customersInLine;
        public LineCustomerStateManager(CustomersManager _customersManager, CustomersOrdersProccesor _customersOrdersProccesor, PizzeriaSceneReferences _sceneReferences)
        {
            customersManager = _customersManager;
            customersOrdersProccesor = _customersOrdersProccesor;
            sceneReferences = _sceneReferences;
            customersInLine = new List<Customer>();
        }
        public void HandleCustomerOfState(Customer customer)
        {
            customer.CustomerAI.OnTargetPointReached += OnCustomerReachedTarget;
            customer.CustomerAI.SetState(CustomerState.InLine);
            customer.CustomerAI.SetTargetPoint(sceneReferences.CustomersPointsInLine[customersInLine.Count]);
            customersInLine.Add(customer);
        }
        void OnCustomerReachedTarget(int customerID)
        {
            Customer firstCustomerInLine = customersInLine[0];
            if (customerID == firstCustomerInLine.Id)
            {
                int orderId = GetPizzaIDForOrder();
                customersOrdersProccesor.ProccesOrderForCustomer(orderId, firstCustomerInLine, OnCustomerDidOrder);
                firstCustomerInLine.SetOrder(orderId);
                firstCustomerInLine.CustomerAI.SetState(CustomerState.MakesOrder);
                firstCustomerInLine.CustomerAI.OnTargetPointReached -= OnCustomerReachedTarget;
                OnCustomerStartOrder?.Invoke(firstCustomerInLine, orderId);
            }
        }
        void OnCustomerDidOrder()
        {
            Customer customer = customersInLine[0];
            customersInLine.RemoveAt(0);
            OnCustomerMadeOrder?.Invoke(customer, customer.OrderId);
            customersManager.SwitchCustomerStateManager(customer, CustomerState.WaitesOrder);
            for (int i = 0; i < customersInLine.Count; i++)
            {
                customersInLine[i].CustomerAI.SetTargetPoint(sceneReferences.CustomersPointsInLine[i]);
            }
        }
        int GetPizzaIDForOrder()
        {
            return Random.Range(0, 6);
        }
    }
}
