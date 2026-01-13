using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.PizzeriaSimulator.Computer.App.Market.Visual
{
    class MarketCartItemBar : MonoBehaviour
    {
        public virtual event Action OnPlus;
        public virtual event Action OnMinus;
        [SerializeField] protected TMP_Text nameTMP;
        [SerializeField] protected TMP_Text priceTMP;
        [SerializeField] protected TMP_Text amountTMP;
        [SerializeField] protected Button minusBtn;
        [SerializeField] protected Button plusBtn;
        protected virtual void Awake()
        {
            minusBtn.onClick.AddListener(OnMinusBtn);
            plusBtn.onClick.AddListener(OnPlusBtn);
        }
        protected virtual void OnDestroy()
        {
            OnPlus = null;
            OnMinus = null;
            minusBtn.onClick.RemoveListener(OnMinusBtn);
            plusBtn.onClick.RemoveListener(OnPlusBtn);
        }
        protected virtual void OnPlusBtn()
        {
            OnPlus?.Invoke();
        }
        protected virtual void OnMinusBtn()
        {
            OnMinus?.Invoke();
        }
        public virtual void SetName(string name)
        {
            nameTMP.text = name;
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
