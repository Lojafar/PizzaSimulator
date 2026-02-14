using Game.PizzeriaSimulator.Orders.Items;
using System;
using System.Collections.Generic;

namespace Game.PizzeriaSimulator.Customers.Manager
{
    [Serializable]
    public struct ManagerOrderData
    {
        public int OrderId;
        public List<PizzeriaOrderItemType> OrderItems;
        public ManagerOrderData(int _orderId, List<PizzeriaOrderItemType> _orderItems)
        {
            OrderId = _orderId;
            OrderItems = _orderItems;
        }
    }
    [Serializable]
    public class CustomersManagerData
    {
        public List<ManagerOrderData> CustomersOrders = new();
        public CustomersManagerData() {  }
        public CustomersManagerData(List<ManagerOrderData> _customersOrders)
        {
            CustomersOrders = _customersOrders;
        }
        public CustomersManagerData Clone()
        {
            return new CustomersManagerData(new List<ManagerOrderData>(CustomersOrders));  // deep copy or seal manager data from public
        }
    }
}
