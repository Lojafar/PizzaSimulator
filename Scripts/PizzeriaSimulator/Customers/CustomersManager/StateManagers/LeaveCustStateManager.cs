using System.Collections.Generic;
using UnityEngine;
namespace Game.PizzeriaSimulator.Customers.Manager.StateManager
{
    class LeaveCustStateManager: ICustomerStateManager
    {
        readonly PizzeriaSceneReferences sceneReferences;
        readonly List<Customer> customers;
        public LeaveCustStateManager(PizzeriaSceneReferences _sceneReferences)
        {
            sceneReferences = _sceneReferences;
            customers = new List<Customer>();
        }
        public void ForceCustomer(Customer customer)
        {
            Object.Destroy(customer.gameObject);
        }
        public void HandleCustomerOfState(Customer customer)
        {
            customer.CustomerAI.OnTargetPointReached += OnCustomerLeaved;
            customer.CustomerAI.SetState(CustomerState.Leaves);
            customer.CustomerAI.SetTargetPoint(sceneReferences.PlayerSpawnPoint);
            customers.Add(customer);
        }
        void OnCustomerLeaved(int customerID)
        {
            for (int i = 0; i < customers.Count; i++)
            {
                if (customers[i].Id == customerID) 
                {
                    Customer customer = customers[i];
                    customers.RemoveAt(i);
                    customer.CustomerAI.OnTargetPointReached -= OnCustomerLeaved;
                    customers.Remove(customer);
                    Object.Destroy(customer.gameObject);
                    return;
                }
            }
        }
    }
}
