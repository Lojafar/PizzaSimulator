namespace Game.PizzeriaSimulator.Customers.Manager.StateManager
{
    interface ICustomerStateManager
    {
        public void HandleCustomerOfState(Customer customer);
        public void RemoveCustomer(Customer customer);
        public void ForceCustomer(Customer customer);
    }
}
