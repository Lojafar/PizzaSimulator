using Game.PizzeriaSimulator.Currency;
using Game.PizzeriaSimulator.Delivery;
using Game.PizzeriaSimulator.Delivery.Config;
using Game.PizzeriaSimulator.Wallet;
using Game.Root.ServicesInterfaces;
using System;
using System.Collections.Generic;

namespace Game.PizzeriaSimulator.Computer.App.Market
{
    public class MarketCompApp : ComputerAppBase, IInittable
    {
        public int InitPriority => 10;
        public event Action<int> OnNewItemInAssortment;
        public event Action<int> OnNewItemInCart;
        public event Action<int> OnItemRemovedFromCart;
        public event Action<int, int> OnAmountInCartUpdated;
        public event Action<int, MoneyQuantity> OnPriceInCartUpdated;
        public event Action<MoneyQuantity> OnTotalPriceChanged;
        public event Action OnCartCleared;
        public event Action OnPurchaseProccesed;
        public event Action OnPurchaseFailed;
        readonly PlayerWallet playerWallet;
        readonly PizzeriaDelivery pizzeriaDelivery;
        readonly PizzeriaDeliveryConfig deliveryConfig;
        readonly Dictionary<int, int> itemsInCart;
        MoneyQuantity totalPrice;
        public MarketCompApp(PlayerWallet _playerWallet, PizzeriaDelivery _pizzeriaDelivery, PizzeriaDeliveryConfig _deliveryConfig)
        {
            playerWallet = _playerWallet;
            pizzeriaDelivery = _pizzeriaDelivery;
            deliveryConfig = _deliveryConfig;
            itemsInCart = new Dictionary<int, int>();
        }
        public void Init()
        {
            int itemsAmount = deliveryConfig.ItemsAmount;
            for (int i = 0; i < itemsAmount; i++)
            {
                OnNewItemInAssortment?.Invoke(i);
            }
            OnTotalPriceChanged?.Invoke(totalPrice);
        }
        public DeliveryItemConfig GetItemConfig(int id)
        {
            return deliveryConfig.GetDeliveryItemConfig(id);
        }
        public void AddToCartInput(int itemId, int amount)
        {
            if (amount < 1) return;
            int newAmount = amount;
            if(itemsInCart.TryGetValue(itemId, out int amountInCart) && amountInCart > 0)
            {
                newAmount += amountInCart;
                itemsInCart[itemId] = newAmount;
            }
            else
            {
                itemsInCart[itemId] = newAmount;
                OnNewItemInCart?.Invoke(itemId);
            }
            OnAmountInCartUpdated?.Invoke(itemId, newAmount);
            OnPriceInCartUpdated?.Invoke(itemId, CalcPriceForItem(itemId, newAmount));
            UpdateTotalPrice(CalcPriceForItem(itemId, amount), amount);
        }
        public void RemoveFromCartInput(int itemId, int amount)
        {
            if (amount < 1) return;
            if (itemsInCart.TryGetValue(itemId, out int amountInCart))
            {
                int newAmount = amountInCart - amount;
                if (newAmount <= 0)
                {
                    itemsInCart.Remove(itemId);
                    OnItemRemovedFromCart?.Invoke(itemId);
                }
                else
                {
                    itemsInCart[itemId] = newAmount;
                    OnAmountInCartUpdated?.Invoke(itemId, newAmount);
                    OnPriceInCartUpdated?.Invoke(itemId, CalcPriceForItem(itemId, newAmount));
                }

                UpdateTotalPrice(CalcPriceForItem(itemId, amount), -amount);
            }
        }
        MoneyQuantity CalcPriceForItem(int itemId, int amount)
        {
            if (amount > 0 && deliveryConfig.GetDeliveryItemConfig(itemId) is DeliveryItemConfig itemConfig)
            {
                return itemConfig.Price * amount;
            }
            else
            {
                return new MoneyQuantity();
            }
        }
        void UpdateTotalPrice(MoneyQuantity addedMoney, int modifier)
        {
            totalPrice += addedMoney * modifier;
            OnTotalPriceChanged?.Invoke(totalPrice);
        }
        public void CartBuyInput()
        {
            if (playerWallet.TryTakeMoney(totalPrice))
            {
                foreach (KeyValuePair<int, int> itemInCart in itemsInCart)
                {
                    pizzeriaDelivery.Order(itemInCart.Key, itemInCart.Value);
                }
                ClearCart();
                OnPurchaseProccesed?.Invoke();
            }
            else
            {
                OnPurchaseFailed?.Invoke();
            }
        }
        public void ClearCartInput()
        {
            ClearCart();
        }
        void ClearCart()
        {
            totalPrice = new MoneyQuantity();
            OnTotalPriceChanged?.Invoke(totalPrice);
            itemsInCart.Clear();
            OnCartCleared?.Invoke();
        }
    }
}
