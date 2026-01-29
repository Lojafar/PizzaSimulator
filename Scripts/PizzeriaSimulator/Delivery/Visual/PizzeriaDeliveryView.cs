using UnityEngine;
using TMPro;
using Game.Root.Utils.Audio;
namespace Game.PizzeriaSimulator.Delivery.Visual
{
    class PizzeriaDeliveryView : PizzeriaDeliveryViewBase
    {
        [SerializeField] AudioClip deliveryEndSFX;
        [SerializeField] TMP_Text timeTMP;
        public override void Bind(PizzeriaDeliveryVM _viewModel)
        {
            base.Bind(_viewModel);
            viewModel.ShowDeliveryVisuals += ShowDeliveryVisual;
            viewModel.HideDeliveryVisuals += HideDeliveryVisual;
            viewModel.UpdateDeliveryTime += UpdateTimeText;
            viewModel.OnDeliveryEnded += OnDeliveryEnded;
        }
        void OnDestroy()
        {
            viewModel.ShowDeliveryVisuals -= ShowDeliveryVisual;
            viewModel.HideDeliveryVisuals -= HideDeliveryVisual;
            viewModel.UpdateDeliveryTime -= UpdateTimeText;
            viewModel.OnDeliveryEnded -= OnDeliveryEnded;
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
        void OnDeliveryEnded()
        {
            AudioPlayer.PlaySFX(deliveryEndSFX);
        }
    }
}
