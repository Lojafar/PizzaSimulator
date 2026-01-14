using UnityEngine;
using TMPro;
namespace Game.PizzeriaSimulator.Delivery.Visual
{
    class PizzeriaDeliveryView : PizzeriaDeliveryViewBase
    {
        [SerializeField] TMP_Text timeTMP;
        public override void Bind(PizzeriaDeliveryVM _viewModel)
        {
            base.Bind(_viewModel);
            viewModel.ShowDeliveryVisuals += ShowDeliveryVisual;
            viewModel.HideDeliveryVisuals += HideDeliveryVisual;
            viewModel.UpdateDeliveryTime += UpdateTimeText;
        }
        void OnDestroy()
        {
            viewModel.ShowDeliveryVisuals -= ShowDeliveryVisual;
            viewModel.HideDeliveryVisuals -= HideDeliveryVisual;
            viewModel.UpdateDeliveryTime -= UpdateTimeText;
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
    }
}
