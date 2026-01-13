using Game.PizzeriaSimulator.Customers.SettingsConfig;
using Game.PizzeriaSimulator.OrdersHandle;
using Game.PizzeriaSimulator.PaymentReceive;
using Game.PizzeriaSimulator.PizzasConfig;
using Game.PizzeriaSimulator.Currency;
using System;
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
        public CustomersOrdersProccesor(PaymentReceiver _paymentReceiver, PizzeriaOrdersHandler _ordersHandler, AllPizzaConfig _allPizzaConfig, CustomersSettingsConfig _settingsConfig)
        {
            paymentReceiver = _paymentReceiver;
            ordersHandler = _ordersHandler;
            allPizzaConfig = _allPizzaConfig;
            settingsConfig = _settingsConfig;
        }
        public MoneyQuantity ProccesOrderForCustomer(int orderID, Customer customer, Action onOrdered)
        {
            int rnd = Random.Range(0, 101);
            PaymentType paymentType = (rnd < settingsConfig.CardPaymentPercent) ? PaymentType.Card : PaymentType.Cash;
            MoneyQuantity price = GetOrderConfig(orderID).Price;
            paymentReceiver.ProccesPayment(paymentType, price, () => OnPaymentProccesed(orderID, onOrdered));
            OnCustomerStartPaying?.Invoke(customer, paymentType, price);
            return allPizzaConfig.GetPizzaByID(orderID).Price;
        }
        void OnPaymentProccesed(int pizzaID, Action onOrdered)
        {
            ordersHandler.Order(pizzaID);
            onOrdered?.Invoke();
        }
        public PizzaConfig GetOrderConfig(int orderID)
        {
            return allPizzaConfig.GetPizzaByID(orderID);
        }
    }
}
