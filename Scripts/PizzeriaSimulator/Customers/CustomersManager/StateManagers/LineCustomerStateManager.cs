using Game.PizzeriaSimulator.Customers.OrdersProcces;
using System;
using System.Collections.Generic;
using UnityEngine;
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
        public void ForceCustomer(Customer customer)
        {
            Transform pointInLine = sceneReferences.CustomersPointsInLine.Length > customersInLine.Count ?
                sceneReferences.CustomersPointsInLine[customersInLine.Count] : sceneReferences.CustomersPointsInLine[0];
            customer.transform.SetPositionAndRotation(pointInLine.position, pointInLine.rotation);
            customer.CustomerAI.SetTargetPoint(pointInLine);
            customer.CustomerAI.SetState(CustomerState.InLine);
            customersInLine.Add(customer);
            OnCustomerReachedTarget(customer.Id);
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
        int GetPizzaIDForOrder()
        {
            return Random.Range(0, 6);
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
        
    }
}
