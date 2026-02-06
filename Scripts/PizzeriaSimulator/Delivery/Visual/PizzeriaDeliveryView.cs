using Game.Root.Utils;
using Game.Root.Utils.Audio;
using Game.PizzeriaSimulator.Delivery.Visual.SubView;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using R3;
namespace Game.PizzeriaSimulator.Delivery.Visual
{
    class PizzeriaDeliveryView : PizzeriaDeliveryViewBase
    {
        [SerializeField] Button gemsSkipButton;
        [SerializeField] Button advSkipButton;
        [SerializeField] AudioClip deliveryEndSFX;
        [SerializeField] TMP_Text timeTMP;
        [SerializeField] TMP_Text gemsSkipCostTMP;
        [SerializeField] DeliverySubViewBase mobileSubView;
        [SerializeField] DeliverySubViewBase pcSubView;
        DeliverySubViewBase currentSubView;
        public override void Bind(PizzeriaDeliveryVM _viewModel)
        {
            base.Bind(_viewModel);
            viewModel.ShowDeliveryVisuals += ShowDeliveryVisual;
            viewModel.HideDeliveryVisuals += HideDeliveryVisual;
            viewModel.UpdateDeliveryTime += UpdateTimeText;
            viewModel.UpdateGemsSkipCostText += UpdateGemsCostText;
            viewModel.OnDeliveryEnded += OnDeliveryEnded;
            viewModel.ShowDeliveryAddTime += ShowDeliveryAddedTime;
            viewModel.ShowGemsSkipFail += ShowFailToast;
            if(viewModel.DeviceType == Root.User.Environment.DeviceType.PC)
            {
                currentSubView = pcSubView;
                Destroy(mobileSubView);
            }
            else
            {
                currentSubView = mobileSubView;
                Destroy(pcSubView);
            }
            currentSubView.GemsSkipInput += OnGemsSkipInput;
            currentSubView.AdvSkipInput += OnAdvSkipInput;
            currentSubView.StartUse();
        }
        void OnDestroy()
        {
            viewModel.ShowDeliveryVisuals -= ShowDeliveryVisual;
            viewModel.HideDeliveryVisuals -= HideDeliveryVisual;
            viewModel.UpdateDeliveryTime -= UpdateTimeText;
            viewModel.UpdateGemsSkipCostText -= UpdateGemsCostText;
            viewModel.OnDeliveryEnded -= OnDeliveryEnded;
            viewModel.ShowDeliveryAddTime -= ShowDeliveryAddedTime;
            viewModel.ShowGemsSkipFail -= ShowFailToast; 
            currentSubView.GemsSkipInput -= OnGemsSkipInput;
            currentSubView.AdvSkipInput -= OnAdvSkipInput;
        }
        void OnGemsSkipInput()
        {
            viewModel.GemsSkipInput.OnNext(Unit.Default);
        }
        void OnAdvSkipInput()
        {
            viewModel.AdvSkipInput.OnNext(Unit.Default);
        }
        void ShowDeliveryVisual()
        {
            gameObject.SetActive(true);
        }
        void HideDeliveryVisual()
        {
            gameObject.SetActive(false);
        }
        void UpdateTimeText(string timeText)
        {
            timeTMP.text = timeText;
        }
        void UpdateGemsCostText(string gemsText)
        {
            gemsSkipCostTMP.text = gemsText;
        }
        void ShowDeliveryAddedTime(string addedTime)
        {
            Toasts.ShowToast(addedTime);
        }
        void OnDeliveryEnded()
        {
            AudioPlayer.PlaySFX(deliveryEndSFX);
        }
        void ShowFailToast(string message)
        {
            AudioPlayer.PlaySFX("Wrong");
            Toasts.ShowToast(message);
        }
    }
}
