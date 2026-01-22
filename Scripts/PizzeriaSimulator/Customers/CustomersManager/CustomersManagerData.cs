using System;
using System.Collections.Generic;

namespace Game.PizzeriaSimulator.Customers.Manager
{
    [Serializable]
    public class CustomersManagerData
    {
        public List<int> CustomersOrders = new();
        public CustomersManagerData() { }
        public CustomersManagerData(List<int> _customersOrders)
        {
            CustomersOrders = _customersOrders;
        }
        public CustomersManagerData Clone()
        {
            return new CustomersManagerData(new List<int>(CustomersOrders));
        }
    }
}
