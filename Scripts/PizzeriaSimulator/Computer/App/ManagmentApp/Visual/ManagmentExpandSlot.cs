using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PizzeriaSimulator.Computer.App.ManagmentApp.Visual
{
    sealed class ManagmentExpandSlot : MonoBehaviour
    {
        public event Action<int> OnPurchaseInput;
        [SerializeField] Button purchaseBtn;
        [SerializeField] TMP_Text nameTMP;
        [SerializeField] TMP_Text descriptionTMP;
        [SerializeField] TMP_Text priceTMP;
        [SerializeField] Image iconImage;
        [SerializeField] GameObject activatedVisuals;
        int id;
        private void Awake()
        {
            activatedVisuals.SetActive(false);
            purchaseBtn.gameObject.SetActive(true);
            purchaseBtn.onClick.AddListener(OnPurchaseBtn);
        }
        private void OnDestroy()
        {
            purchaseBtn.onClick.RemoveListener(OnPurchaseBtn);
            OnPurchaseInput = null;
        }
        void OnPurchaseBtn()
        {
            OnPurchaseInput?.Invoke(id);
        }
        public void SetId(int _id)
        {
            id = _id;
        }
        public void SetNameText(string name)
        {
            nameTMP.text = name;
        }
        public void SetDescription(string description) 
        {
            descriptionTMP.text = description;
        }
        public void SetPriceText(string priceText) 
        {
            priceTMP.text = priceText;
        }
        public void SetIcon(Sprite icon) 
        {
            iconImage.sprite = icon;
        }
        public void Activate()
        {
            activatedVisuals.SetActive(true);
            purchaseBtn.gameObject.SetActive(false);
        }
    }
}
