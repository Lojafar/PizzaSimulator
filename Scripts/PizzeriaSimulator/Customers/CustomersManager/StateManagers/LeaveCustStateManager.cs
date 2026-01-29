using System;
using System.Collections.Generic;
namespace Game.PizzeriaSimulator.Customers.Manager.StateManager
{
    class LeaveCustStateManager : ICustomerStateManager
    {
        public event Action<Customer> OnCustomerLeaved;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly List<Customer> customers;
        public LeaveCustStateManager(PizzeriaSceneReferences _sceneReferences)
        {
            sceneReferences = _sceneReferences;
            customers = new List<Customer>();
        }
        public void ForceCustomer(Customer customer)
        {
            OnCustomerLeaved?.Invoke(customer);
        }
        public void HandleCustomerOfState(Customer customer)
        {
            customer.CustomerAI.OnTargetPointReached += CustomerReachedLeave;
            customer.CustomerAI.SetState(CustomerState.Leaves);
            customer.CustomerAI.SetTargetPoint(sceneReferences.PlayerSpawnPoint);
            customers.Add(customer);
        }
        void CustomerReachedLeave(int customerID)
        {
            for (int i = 0; i < customers.Count; i++)
            {
                if (customers[i].Id == customerID)
                {
                    Customer customer = customers[i];
                    customers.RemoveAt(i);
                    customer.CustomerAI.OnTargetPointReached -= CustomerReachedLeave;
                    OnCustomerLeaved?.Invoke(customer);
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
                    customer.CustomerAI.OnTargetPointReached -= CustomerReachedLeave;
                }
            }
        }
    }
}
