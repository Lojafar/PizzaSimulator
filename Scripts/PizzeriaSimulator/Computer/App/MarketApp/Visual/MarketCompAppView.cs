using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.PizzeriaSimulator.Computer.App.Market.Visual
{
    class MarketCompAppView : MarketCompAppViewBase
    {
        [SerializeField] GameObject appPanel;
        [SerializeField] Button closeBtn;
        [SerializeField] Button cartBuyBtn;
        [SerializeField] Button cartClearBtn;
        [SerializeField] Transform ingredientsContainer;
        [SerializeField] Transform cartBarsContainer;
        [SerializeField] MarketItemCard itemCardPrefab;
        [SerializeField] MarketCartItemBar cartItemBarPrefab;
        [SerializeField] TMP_Text totalPriceTMP;
        readonly Dictionary<int, MarketCartItemBar> idToCartBar = new();
        public override void Bind(MarketCompAppVM _viewModel)
        {
            base.Bind(_viewModel);
            closeBtn.onClick.AddListener(OnCloseBtn);
            cartBuyBtn.onClick.AddListener(OnCartBuyBtn);
            cartClearBtn.onClick.AddListener(OnCartClearBtn);
            viewModel.Open += OnOpen;
            viewModel.Close += OnClose;
            viewModel.OnNewItem += SpawnNewItemCard;
            viewModel.OnNewCartItem += SpawnNewCartBar;
            viewModel.UpdateCartItemAmount += UpdateCartBarAmount;
            viewModel.UpdateCartItemPrice += UpdateCartBarPrice;
            viewModel.RemoveCartItem += RemoveCartBar;
            viewModel.RemoveAllCartItems += DestroyAllBars;
            viewModel.UpdateTotalPriceText += UpdateTotalPriceText;
        }
        private void OnDestroy()
        {
            closeBtn.onClick.RemoveListener(OnCloseBtn);
            cartBuyBtn.onClick.RemoveListener(OnCartBuyBtn);
            cartClearBtn.onClick.RemoveListener(OnCartClearBtn);
            if (viewModel != null)
            {
                viewModel.Open -= OnOpen;
                viewModel.Close -= OnClose;
                viewModel.OnNewItem -= SpawnNewItemCard;
                viewModel.OnNewCartItem -= SpawnNewCartBar;
                viewModel.UpdateCartItemAmount -= UpdateCartBarAmount;
                viewModel.UpdateCartItemPrice -= UpdateCartBarPrice;
                viewModel.RemoveCartItem -= RemoveCartBar;
                viewModel.RemoveAllCartItems -= DestroyAllBars;
                viewModel.UpdateTotalPriceText -= UpdateTotalPriceText;
            }
        }
        void OnCloseBtn()
        {
            viewModel.CloseInput();
        }
        void OnCartBuyBtn()
        {
            viewModel.CartBuyInput();
        }
        void OnCartClearBtn()
        {
            viewModel.CartClearInput();
        }
        void OnOpen()
        {
            appPanel.SetActive(true);
        }
        void OnClose()
        {
            appPanel.SetActive(false);
        }
        void SpawnNewItemCard(MarketItemViewData itemData)
        {
            MarketItemCard spawnedCard = Instantiate(itemCardPrefab, ingredientsContainer);
            spawnedCard.SetItemName(itemData.Name);
            spawnedCard.SetAmountText(itemData.AmountText);
            spawnedCard.SetPriceText(itemData.PriceText);
            spawnedCard.SetItemIcon(itemData.ItemIcon);
            spawnedCard.OnAddToCartBtn += () => OnCardInput(itemData.Id);
        }
        void OnCardInput(int itemId)
        {
            viewModel.PlusItemInCart(itemId);
        }
        void SpawnNewCartBar(int itemId, string name)
        {
            MarketCartItemBar spawnedBar = Instantiate(cartItemBarPrefab, cartBarsContainer);
            spawnedBar.SetName(name);
            spawnedBar.OnMinus += () => OnMinusBarInput(itemId);
            spawnedBar.OnPlus += () => OnPlusBarInput(itemId);
            idToCartBar.Add(itemId, spawnedBar);
        }
        void OnMinusBarInput(int itemId)
        {
            viewModel.MinusItemInCart(itemId);
        }
        void OnPlusBarInput(int itemId)
        {
            viewModel.PlusItemInCart(itemId);
        }
        void UpdateCartBarAmount(int barId, string amountText)
        {
            if (idToCartBar.TryGetValue(barId, out MarketCartItemBar cartBar))
            {
                cartBar.SetAmountText(amountText);
            }
        }
        void UpdateCartBarPrice(int barId, string priceText)
        {
            if (idToCartBar.TryGetValue(barId, out MarketCartItemBar cartBar))
            {
                cartBar.SetPriceText(priceText);
            }
        }
        void RemoveCartBar(int barId)
        {
            if (idToCartBar.TryGetValue(barId, out MarketCartItemBar cartBar))
            {
                Destroy(cartBar.gameObject);
                idToCartBar.Remove(barId);
            }
        }
        void DestroyAllBars()
        {
            foreach (MarketCartItemBar cartBar in idToCartBar.Values)
            {
                Destroy(cartBar.gameObject);
            }
            idToCartBar.Clear();
        }
        void UpdateTotalPriceText(string priceText)
        {
            totalPriceTMP.text = priceText;
        }
    }
}
