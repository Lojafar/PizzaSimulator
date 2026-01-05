namespace Game.PizzeriaSimulator.Customers.Visual
{
    interface ICustomerVisualHandler
    {
        public void UpdateCustomerState(CustomerState customerState);
        public void UpdateMoveSpeed(float currentSpeed);
    }
}
