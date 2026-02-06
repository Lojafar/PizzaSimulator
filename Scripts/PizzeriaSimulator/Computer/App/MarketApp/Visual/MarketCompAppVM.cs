using Game.PizzeriaSimulator.Currency;
using Game.PizzeriaSimulator.Delivery.Config;
using Game.Root.ServicesInterfaces;
using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Computer.App.Market.Visual
{
    public class MarketItemViewData
    {
        public readonly int Id;
        public readonly string Name;
        public readonly string PriceText;
        public readonly string AmountText;
        public readonly Sprite ItemIcon;
        public MarketItemViewData(int _id, string _name, string _priceText, string _amountText, Sprite _icon)
        {
            Id = _id;
            Name = _name;
            PriceText = _priceText;
            AmountText = _amountText;
            ItemIcon = _icon;
        }
    }

    public class MarketCompAppVM : ISceneDisposable
    {
        public event Action Open;
        public event Action Close;
        public event Action RemoveAllCartItems;
        public event Action ShowPurchaseSucces;
        public event Action<bool> ActivateIngredientPage;
        public event Action<bool> ActivateFurniturePage;
        public event Action<MarketItemViewData> OnNewItemInIngredient;
        public event Action<MarketItemViewData> OnNewItemInFurniture;
        public event Action<int, string> OnNewCartItem;
        public event Action<int, string> UpdateCartItemAmount;
        public event Action<int, string> UpdateCartItemPrice;
        public event Action<int> RemoveCartItem;
        public event Action<string> UpdateTotalPriceText;
        public event Action<string> ShowPurchaseFail;
        readonly MarketCompApp marketCompApp;
        const string pricePattern = "${0}.{1:D2}";
        const string amountPatter = "{0}";
        public MarketCompAppVM(MarketCompApp _marketCompApp)
        {
            marketCompApp = _marketCompApp;
        }
        public void Init()
        {
            marketCompApp.OnOpen += HandleOpen;
            marketCompApp.OnClose += HandleClose;
            marketCompApp.OnPageOpened += HandlePageOpen;
            marketCompApp.OnPageClosed += HandlePageClose;
            marketCompApp.OnNewItemInAssortment += HandleNewItem;
            marketCompApp.OnNewItemInCart += HandleNewCartItem;
            marketCompApp.OnAmountInCartUpdated += HandleCartItemAmount;
            marketCompApp.OnPriceInCartUpdated += HandleCartItemPrice;
            marketCompApp.OnItemRemovedFromCart += HandleCartItemRemove;
            marketCompApp.OnCartCleared += HandleCartClear;
            marketCompApp.OnTotalPriceChanged += HandleNewTotalPrice;
            marketCompApp.OnPurchaseProccesed += HandlePurchaseSucces;
            marketCompApp.OnPurchaseFailed += HandlePurchaseFail;
        }
        public void Dispose()
        {
            marketCompApp.OnOpen -= HandleOpen;
            marketCompApp.OnClose -= HandleClose;
            marketCompApp.OnPageOpened -= HandlePageOpen;
            marketCompApp.OnPageClosed -= HandlePageClose;
            marketCompApp.OnNewItemInAssortment -= HandleNewItem;
            marketCompApp.OnNewItemInCart -= HandleNewCartItem;
            marketCompApp.OnAmountInCartUpdated -= HandleCartItemAmount;
            marketCompApp.OnPriceInCartUpdated -= HandleCartItemPrice;
            marketCompApp.OnItemRemovedFromCart -= HandleCartItemRemove;
            marketCompApp.OnCartCleared -= HandleCartClear;
            marketCompApp.OnTotalPriceChanged -= HandleNewTotalPrice;
            marketCompApp.OnPurchaseProccesed -= HandlePurchaseSucces;
            marketCompApp.OnPurchaseFailed -= HandlePurchaseFail;
        }
        public void CloseInput()
        {
            marketCompApp.Close();
        }
        public void IngredientPageInput()
        {
            marketCompApp.OpenPageInput(MarketAppPageType.IngredientsPage);
        }
        public void FurniturePageInput()
        {
            marketCompApp.OpenPageInput(MarketAppPageType.FurniturePage);
        }
        public void CartClearInput()
        {
            marketCompApp.ClearCartInput();
        }
        public void CartBuyInput()
        {
            marketCompApp.CartBuyInput();
        }
        public void PlusItemInCart(int itemId)
        {
            marketCompApp.AddToCartInput(itemId, 1);
        }
        public void MinusItemInCart(int itemId)
        {
            marketCompApp.RemoveFromCartInput(itemId, 1);
        }
        void HandleOpen()
        {
            Open?.Invoke();
        }
        void HandleClose()
        {
            Close?.Invoke();
        }
        void HandlePageOpen(MarketAppPageType pageType)
        {
            switch (pageType)
            {
                case MarketAppPageType.IngredientsPage:
                    ActivateIngredientPage?.Invoke(true);
                    break;
                case MarketAppPageType.FurniturePage:
                    ActivateFurniturePage?.Invoke(true);
                    break;
            }
        }
        void HandlePageClose(MarketAppPageType pageType)
        {
            switch (pageType)
            {
                case MarketAppPageType.IngredientsPage:
                    ActivateIngredientPage?.Invoke(false);
                    break;
                case MarketAppPageType.FurniturePage:
                    ActivateFurniturePage?.Invoke(false);
                    break;
            }
        }
        void HandleNewItem(int itemID, MarketAppPageType pageType)
        {
            if (marketCompApp.GetItemConfig(itemID) is DeliveryItemConfig itemConfig)
            {
                MarketItemViewData marketItemViewData = new(itemConfig.ID, itemConfig.Name,
                    GetPriceText(itemConfig.Price), GetItemAmountText(itemConfig.QuantityByOrder), itemConfig.ItemIcon);
                switch (pageType)
                {
                    case MarketAppPageType.IngredientsPage:
                        OnNewItemInIngredient?.Invoke(marketItemViewData);
                        break;
                    case MarketAppPageType.FurniturePage:
                        OnNewItemInFurniture?.Invoke(marketItemViewData);
                        break;
                }
            }
        }
        string GetPriceText(MoneyQuantity moneyQuantity)
        {
            return string.Format(pricePattern, moneyQuantity.Dollars, moneyQuantity.Cents);
        }
        string GetItemAmountText(int amount)
        {
            return string.Format(amountPatter, amount);
        }
        void HandleNewCartItem(int itemID)
        {
            if (marketCompApp.GetItemConfig(itemID) is DeliveryItemConfig itemConfig)
            {
                OnNewCartItem?.Invoke(itemID, itemConfig.Name);
            }
        }
        void HandleCartItemAmount(int itemID, int amount)
        {
            UpdateCartItemAmount?.Invoke(itemID, GetItemAmountText(amount));
        }
        void HandleCartItemPrice(int itemID, MoneyQuantity price)
        {
            UpdateCartItemPrice?.Invoke(itemID, GetPriceText(price));
        }
        void HandleCartItemRemove(int itemID)
        {
            RemoveCartItem?.Invoke(itemID);
        }
        void HandleCartClear()
        {
            RemoveAllCartItems?.Invoke();
        }
        void HandleNewTotalPrice(MoneyQuantity newTotal)
        {
            UpdateTotalPriceText?.Invoke(GetPriceText(newTotal));
        }
        void HandlePurchaseSucces()
        {
            ShowPurchaseSucces?.Invoke();
        }
        void HandlePurchaseFail()
        {
            ShowPurchaseFail?.Invoke("Not enough money");
        }
    }
}
