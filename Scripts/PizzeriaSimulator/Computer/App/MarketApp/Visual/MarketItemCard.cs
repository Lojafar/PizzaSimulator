using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Game.PizzeriaSimulator.Computer.App.Market.Visual
{
    class MarketItemCard : MonoBehaviour
    {
        public event Action OnAddToCartBtn;
        [SerializeField] protected Image itemIconImage;
        [SerializeField] protected TMP_Text itemNameTMP;
        [SerializeField] protected TMP_Text priceTMP;
        [SerializeField] protected TMP_Text amountTMP;
        [SerializeField] protected Button toCartBtn;
        [SerializeField] protected RectTransform priceContainer;

        protected virtual void Awake()
        {
            toCartBtn.onClick.AddListener(OnToCartBtn);
        }
        protected virtual void OnDestroy()
        {
            toCartBtn.onClick.RemoveListener(OnToCartBtn);
            OnAddToCartBtn = null;
        }
        protected virtual void OnToCartBtn()
        {
            OnAddToCartBtn?.Invoke();
        }
        public virtual void SetItemIcon(Sprite icon)
        {
            itemIconImage.sprite = icon;
        }
        public virtual void SetItemName(string name) 
        {
            itemNameTMP.text = name;
        }
        public virtual void SetPriceText(string priceText) 
        {
            priceTMP.text = priceText;
        }
        public virtual void SetAmountText(string amountText) 
        {
            amountTMP.text = amountText;
        }

    }
}
