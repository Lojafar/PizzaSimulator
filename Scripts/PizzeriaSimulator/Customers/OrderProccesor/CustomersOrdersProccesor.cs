using Game.PizzeriaSimulator.Customers.SettingsConfig;
using Game.PizzeriaSimulator.Orders.Handle;
using Game.PizzeriaSimulator.Orders.Items;
using Game.PizzeriaSimulator.PaymentReceive;
using Game.PizzeriaSimulator.PizzasConfig;
using Game.PizzeriaSimulator.Currency;
using System;
using System.Collections.Generic;
namespace Game.PizzeriaSimulator.Customers.OrdersProcces
{
    using Random = UnityEngine.Random;
    public class CustomersOrdersProccesor 
    {
        public event Action<Customer, PaymentType, MoneyQuantity> OnCustomerStartPaying;
        readonly PaymentReceiver paymentReceiver;
        readonly PizzeriaOrdersHandler ordersHandler;
        readonly AllPizzaConfig allPizzaConfig;
        readonly CustomersSettingsConfig settingsConfig;
        readonly Dictionary<int, List<PizzeriaOrderItemType>> orderItemsById;
        public CustomersOrdersProccesor(PaymentReceiver _paymentReceiver, PizzeriaOrdersHandler _ordersHandler, AllPizzaConfig _allPizzaConfig, CustomersSettingsConfig _settingsConfig)
        {
            paymentReceiver = _paymentReceiver;
            ordersHandler = _ordersHandler;
            allPizzaConfig = _allPizzaConfig;
            settingsConfig = _settingsConfig;
            orderItemsById = new Dictionary<int, List<PizzeriaOrderItemType>>();
        }
      
        public void DisposeOrder(int orderId)
        {
            if (orderItemsById.ContainsKey(orderId)) 
            {
                orderItemsById.Remove(orderId);
            }
        }
        public bool TryGetOrderItems(int orderID, out List<PizzeriaOrderItemType> items)
        {
            return orderItemsById.TryGetValue(orderID, out items);
        }
        public int ForceCustomerOrder(List<PizzeriaOrderItemType> orderItems)
        {
            if (orderItems == null) return -1;
            int orderId = ordersHandler.Order(orderItems);
            orderItemsById.Add(orderId, orderItems);
            return orderId;
        }
        public void ProccesOrderForCustomer(int pizzaId, Customer customer, Action onOrdered)
        {
            int rnd = Random.Range(0, 101);
            PaymentType paymentType = (rnd < settingsConfig.CardPaymentPercent) ? PaymentType.Card : PaymentType.Cash;
            MoneyQuantity price = GetOrderConfig(pizzaId).Price;

            /// for test
            int reservedOrderId = ordersHandler.ReserveOrderID();
            List<PizzeriaOrderItemType> orderItems = Random.Range(0, 6) < 10 ? new List<PizzeriaOrderItemType> {
                pizzaId switch
            {
                0 => PizzeriaOrderItemType.CheesePizza,
                1 => PizzeriaOrderItemType.Pepperoni,
                2 => PizzeriaOrderItemType.TomatoPepperoni,
                3 => PizzeriaOrderItemType.ShrimpPizza,
                4 => PizzeriaOrderItemType.VillagePizza,
                5 => PizzeriaOrderItemType.VegeterianPizza,
                _ => PizzeriaOrderItemType.CheesePizza
            }, PizzeriaOrderItemType.Soda} : new System.Collections.Generic.List<PizzeriaOrderItemType> {  Random.Range(0, 6) switch
            {
                0 => PizzeriaOrderItemType.CheesePizza,
                1 => PizzeriaOrderItemType.Pepperoni,
                2 => PizzeriaOrderItemType.TomatoPepperoni,
                3 => PizzeriaOrderItemType.ShrimpPizza,
                4 => PizzeriaOrderItemType.VillagePizza,
                5 => PizzeriaOrderItemType.VegeterianPizza,
                _ => PizzeriaOrderItemType.CheesePizza
            },pizzaId switch
            {
                0 => PizzeriaOrderItemType.CheesePizza,
                1 => PizzeriaOrderItemType.Pepperoni,
                2 => PizzeriaOrderItemType.TomatoPepperoni,
                3 => PizzeriaOrderItemType.ShrimpPizza,
                4 => PizzeriaOrderItemType.VillagePizza,
                5 => PizzeriaOrderItemType.VegeterianPizza,
                _ => PizzeriaOrderItemType.CheesePizza
            }};
            /// /for test

            orderItemsById.Add(reservedOrderId, orderItems);
            customer.SetOrder(reservedOrderId);
            paymentReceiver.ProccesPayment(paymentType, price, () => OnPaymentProccesed(reservedOrderId, onOrdered));
            OnCustomerStartPaying?.Invoke(customer, paymentType, price);
        }
        void OnPaymentProccesed(int orderId, Action onOrdered)
        {
            if(orderItemsById.TryGetValue(orderId, out List<PizzeriaOrderItemType> orderItems))
            {
                ordersHandler.OrderWithReservedId(orderId, orderItems);
                onOrdered?.Invoke();
            } 
        }
        public PizzaConfig GetOrderConfig(int orderID)
        {
            return allPizzaConfig.GetPizzaByID(orderID);
        }
    }
}
