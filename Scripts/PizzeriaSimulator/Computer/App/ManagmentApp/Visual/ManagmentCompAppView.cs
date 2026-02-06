using Game.Root.Utils;
using Game.Root.Utils.Audio;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Game.PizzeriaSimulator.Computer.App.ManagmentApp.Visual
{
    sealed class ManagmentCompAppView : ManagCompAppViewBase
    {
        [SerializeField] GameObject appPanel;
        [SerializeField] Button closeBtn;
        [SerializeField] Transform expandSlotsContainer;
        [SerializeField] ManagmentExpandSlot expandSlotPrefab;
        readonly List<ManagmentExpandSlot> expandSlots = new();
        public override void Bind(ManagmentCompAppVM _viewModel)
        {
            closeBtn.onClick.AddListener(OnCloseBtn);
            base.Bind(_viewModel);
            viewModel.Open += OnOpen;
            viewModel.Close += OnClose;
            viewModel.AddNewExpansionItem += SpawnNewExpandSlot;
            viewModel.ChangeExpansSlotOrder += SetOrderToExpansionSlot;
            viewModel.ActivateExpansSlot += ActivateExpansSlot;
            viewModel.ShowExpansFailMessage += ShowExpandPurchaseFail;
        }
        private void OnDestroy()
        {
            closeBtn.onClick.RemoveListener(OnCloseBtn);
            if (viewModel != null)
            {
                viewModel.Open -= OnOpen;
                viewModel.Close -= OnClose;
                viewModel.AddNewExpansionItem -= SpawnNewExpandSlot;
                viewModel.ChangeExpansSlotOrder -= SetOrderToExpansionSlot;
                viewModel.ActivateExpansSlot -= ActivateExpansSlot;
                viewModel.ShowExpansFailMessage -= ShowExpandPurchaseFail;
            }
            for (int i = 0; i < expandSlots.Count; i++)
            {
                expandSlots[i].OnPurchaseInput -= OnPurchaseExpandInput;
            }
        }
        void OnCloseBtn()
        {
            viewModel.CloseInput();
        }
        void OnOpen()
        {
            appPanel.SetActive(true);
        }
        void OnClose()
        {
            appPanel.SetActive(false);
        }
        void SpawnNewExpandSlot(ManagmentExpansViewData slotData)
        {
            ManagmentExpandSlot spawnedSlot = Instantiate(expandSlotPrefab, expandSlotsContainer);
            spawnedSlot.SetId(slotData.Id);
            spawnedSlot.SetNameText(slotData.Name);
            spawnedSlot.SetDescription(slotData.Description);
            spawnedSlot.SetPriceText(slotData.PriceText);
            spawnedSlot.SetIcon(slotData.Icon);
            spawnedSlot.OnPurchaseInput += OnPurchaseExpandInput;
            expandSlots.Add(spawnedSlot);
        }
        void OnPurchaseExpandInput(int expansionId)
        {
            viewModel.PurchaseExpansInput(expansionId);
        }
        void SetOrderToExpansionSlot(int slotId, int order)
        {
            if (slotId < 0 || slotId >= expandSlots.Count) return;
            expandSlots[slotId].transform.SetSiblingIndex(order);
        }
        void ActivateExpansSlot(int slotId)
        {
            if (slotId < 0 || slotId >= expandSlots.Count) return;
            expandSlots[slotId].Activate();
        }
        void ShowExpandPurchaseFail(string message)
        {
            AudioPlayer.PlaySFX("Wrong");
            Toasts.ShowToast(message);
        }
    }
}
