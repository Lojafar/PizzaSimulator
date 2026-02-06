using Game.Helps.UI;
using Game.Root.Utils;
using Game.Root.Utils.Audio;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PizzeriaSimulator.Computer.App.Market.Visual
{
    class MarketCompAppView : MarketCompAppViewBase
    {
        [SerializeField] AudioClip clickSFX;
        [SerializeField] GameObject appPanel;
        [SerializeField] GameObject ingredientPage;
        [SerializeField] GameObject furniturePage;
        [SerializeField] Button closeBtn;
        [SerializeField] Button ingredientsPageBtn;
        [SerializeField] Button furniturePageBtn;
        [SerializeField] Button cartBuyBtn;
        [SerializeField] Button cartClearBtn;
        [SerializeField] UISelectTransitionBase ingredsSelectTransit;
        [SerializeField] UISelectTransitionBase furnitSelectTransit;
        [SerializeField] Transform ingredientsContainer;
        [SerializeField] Transform furnitureContainer;
        [SerializeField] Transform cartBarsContainer;
        [SerializeField] MarketItemCard itemCardPrefab;
        [SerializeField] MarketCartItemBar cartItemBarPrefab;
        [SerializeField] TMP_Text totalPriceTMP;
        readonly Dictionary<int, MarketCartItemBar> idToCartBar = new();
        public override void Bind(MarketCompAppVM _viewModel)
        {
            base.Bind(_viewModel);
            closeBtn.onClick.AddListener(OnCloseBtn);
            ingredientsPageBtn.onClick.AddListener(OnIngredientPageInput);
            furniturePageBtn.onClick.AddListener(OnFurniturePageBtn);
            cartBuyBtn.onClick.AddListener(OnCartBuyBtn);
            cartClearBtn.onClick.AddListener(OnCartClearBtn);
            viewModel.Open += OnOpen;
            viewModel.Close += OnClose;
            viewModel.ActivateIngredientPage += ActivateIngredientsPage;
            viewModel.ActivateFurniturePage += ActivateFurniturePage;
            viewModel.OnNewItemInIngredient += SpawnNewIngredientItem;
            viewModel.OnNewItemInFurniture += SpawnNewFurnitureItem;
            viewModel.OnNewCartItem += SpawnNewCartBar;
            viewModel.UpdateCartItemAmount += UpdateCartBarAmount;
            viewModel.UpdateCartItemPrice += UpdateCartBarPrice;
            viewModel.RemoveCartItem += RemoveCartBar;
            viewModel.RemoveAllCartItems += DestroyAllBars;
            viewModel.UpdateTotalPriceText += UpdateTotalPriceText;
            viewModel.ShowPurchaseFail += ShowPurchaseFail;
            viewModel.ShowPurchaseSucces += ShowPurchaseSucces;
        }
        private void OnDestroy()
        {
            closeBtn.onClick.RemoveListener(OnCloseBtn);
            ingredientsPageBtn.onClick.RemoveListener(OnIngredientPageInput);
            furniturePageBtn.onClick.RemoveListener(OnFurniturePageBtn);
            cartBuyBtn.onClick.RemoveListener(OnCartBuyBtn);
            cartClearBtn.onClick.RemoveListener(OnCartClearBtn);
            if (viewModel != null)
            {
                viewModel.Open -= OnOpen;
                viewModel.Close -= OnClose;
                viewModel.ActivateIngredientPage -= ActivateIngredientsPage;
                viewModel.ActivateFurniturePage -= ActivateFurniturePage;
                viewModel.OnNewItemInIngredient -= SpawnNewIngredientItem;
                viewModel.OnNewItemInFurniture -= SpawnNewFurnitureItem;
                viewModel.OnNewCartItem -= SpawnNewCartBar;
                viewModel.UpdateCartItemAmount -= UpdateCartBarAmount;
                viewModel.UpdateCartItemPrice -= UpdateCartBarPrice;
                viewModel.RemoveCartItem -= RemoveCartBar;
                viewModel.RemoveAllCartItems -= DestroyAllBars;
                viewModel.UpdateTotalPriceText -= UpdateTotalPriceText;
                viewModel.ShowPurchaseFail -= ShowPurchaseFail;
                viewModel.ShowPurchaseSucces -= ShowPurchaseSucces;
            }
        }
        void OnCloseBtn()
        {
            AudioPlayer.PlaySFX(clickSFX);
            viewModel.CloseInput();
        }
        void OnIngredientPageInput()
        {
            AudioPlayer.PlaySFX(clickSFX);
            viewModel.IngredientPageInput();
        }
        void OnFurniturePageBtn()
        {
            AudioPlayer.PlaySFX(clickSFX);
            viewModel.FurniturePageInput();
        }
        void OnCartBuyBtn()
        {
            viewModel.CartBuyInput();
        }
        void OnCartClearBtn()
        {
            AudioPlayer.PlaySFX(clickSFX);
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
        void ActivateIngredientsPage(bool activate)
        {
            ingredientPage.SetActive(activate);
            if (activate) ingredsSelectTransit.Select();
            else ingredsSelectTransit.Deselect();
        }
        void ActivateFurniturePage(bool activate)
        {
            furniturePage.SetActive(activate);
            if (activate) furnitSelectTransit.Select();
            else furnitSelectTransit.Deselect();
        }
        void SpawnNewIngredientItem(MarketItemViewData itemData)
        {
            SpawnNewItemCard(itemData, ingredientsContainer);
        }
        void SpawnNewFurnitureItem(MarketItemViewData itemData)
        {
            SpawnNewItemCard(itemData, furnitureContainer);
        }
        void SpawnNewItemCard(MarketItemViewData itemData, Transform parent)
        {
            MarketItemCard spawnedCard = Instantiate(itemCardPrefab, parent);
            spawnedCard.SetItemName(itemData.Name);
            spawnedCard.SetAmountText(itemData.AmountText);
            spawnedCard.SetPriceText(itemData.PriceText);
            spawnedCard.SetItemIcon(itemData.ItemIcon);
            spawnedCard.OnAddToCartBtn += () => OnCardInput(itemData.Id);
        }
        void OnCardInput(int itemId)
        {
            AudioPlayer.PlaySFX(clickSFX);
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
            AudioPlayer.PlaySFX(clickSFX);
            viewModel.MinusItemInCart(itemId);
        }
        void OnPlusBarInput(int itemId)
        {
            AudioPlayer.PlaySFX(clickSFX);
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
        void ShowPurchaseFail(string message)
        {
            AudioPlayer.PlaySFX("Wrong");
            Toasts.ShowToast(message);
        }
        void ShowPurchaseSucces()
        {
            AudioPlayer.PlaySFX(clickSFX);
        }
    }
}
