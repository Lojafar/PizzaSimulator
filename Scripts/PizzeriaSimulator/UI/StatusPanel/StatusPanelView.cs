using UnityEngine;
using TMPro;
namespace Game.PizzeriaSimulator.UI.StatusPanel
{
    class StatusPanelView : StatusPanelViewBase
    {
        [SerializeField] TMP_Text moneyAmountTMP;
        public override void Bind(StatusPanelVM _viewModel)
        {
            base.Bind(_viewModel);
            viewModel.UpdateMoneyText += UpdateMoneyText;
        }
        private void OnDestroy()
        {
            viewModel.UpdateMoneyText -= UpdateMoneyText;
        }
        void UpdateMoneyText(string moneyText)
        {
            moneyAmountTMP.text = moneyText;
        }
    }
}
