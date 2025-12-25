using Game.PizzeriaSimulator.Player.CameraController;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor.Visual
{
    class CardPaymentProccesorView : CardPaymentProccesorViewBase
    {
        [SerializeField] Transform playerCamLookTransform;
        [SerializeField] GameObject cardPaymentPanel;
        [SerializeField] GameObject completePaymentPanel;
        [SerializeField] TMP_Text totalPriceTMP;
        [SerializeField] TMP_Text enterAmountTMP;
        [SerializeField] Button confirmButton;
        [SerializeField] Button backButton;
        [SerializeField] Button dotButton;
        [SerializeField] Button digit1Btn, digit2Btn, digit3Btn, digit4Btn, digit5Btn, digit6Btn, digit7Btn, digit8Btn, digit9Btn, digit0Btn;
        [SerializeField] float completeShowDelay;
        DiContainer diContainer;
        PlayerCameraControllerBase playerCamController;

        [Inject]
        public void Construct(DiContainer _diContainer)
        {
            diContainer = _diContainer;
        }
        public override void Bind(CardPaymentProccesorVM _viewModel)
        {
            playerCamController = diContainer.Resolve<PlayerCameraControllerBase>();
            viewModel = _viewModel;
            confirmButton.onClick.AddListener(OnConfirmBtn);
            backButton.onClick.AddListener(OnBackBtn);
            digit1Btn.onClick.AddListener(OnDigit1Btn);
            digit2Btn.onClick.AddListener(OnDigit2Btn);
            digit3Btn.onClick.AddListener(OnDigit3Btn);
            digit4Btn.onClick.AddListener(OnDigit4Btn);
            digit5Btn.onClick.AddListener(OnDigit5Btn);
            digit6Btn.onClick.AddListener(OnDigit6Btn);
            digit7Btn.onClick.AddListener(OnDigit7Btn);
            digit8Btn.onClick.AddListener(OnDigit8Btn);
            digit9Btn.onClick.AddListener(OnDigit9Btn);
            digit0Btn.onClick.AddListener(OnDigit0Btn);
            dotButton.onClick.AddListener(OnDotBtn);
            viewModel.OnStartProcces += StartProcces;
            viewModel.OnCompleteProcces += OnCompleteProcces;
            viewModel.UpdatePriceText += UpdatePriceText;
            viewModel.UpdateEnterText += UpdateEnterText;
        }
        private void OnDestroy()
        {
            confirmButton.onClick.RemoveListener(OnConfirmBtn);
            backButton.onClick.RemoveListener(OnBackBtn);
            digit1Btn.onClick.RemoveListener(OnDigit1Btn);
            digit2Btn.onClick.RemoveListener(OnDigit2Btn);
            digit3Btn.onClick.RemoveListener(OnDigit3Btn);
            digit4Btn.onClick.RemoveListener(OnDigit4Btn);
            digit5Btn.onClick.RemoveListener(OnDigit5Btn);
            digit6Btn.onClick.RemoveListener(OnDigit6Btn);
            digit7Btn.onClick.RemoveListener(OnDigit7Btn);
            digit8Btn.onClick.RemoveListener(OnDigit8Btn);
            digit9Btn.onClick.RemoveListener(OnDigit9Btn);
            digit0Btn.onClick.RemoveListener(OnDigit0Btn);
            dotButton.onClick.RemoveListener(OnDotBtn);
            if (viewModel != null)
            {
                viewModel.OnStartProcces -= StartProcces;
                viewModel.OnCompleteProcces -= OnCompleteProcces;
                viewModel.UpdatePriceText -= UpdatePriceText;
                viewModel.UpdateEnterText -= UpdateEnterText;
            }
        }
        void OnConfirmBtn()
        {
            viewModel.ConfirmInput.OnNext(Unit.Default);
        }
        void OnBackBtn()
        {
            viewModel.BackInput.OnNext(Unit.Default);
        }
        void OnDigit1Btn() { viewModel.DigitInput.OnNext(1); }
        void OnDigit2Btn() { viewModel.DigitInput.OnNext(2); }
        void OnDigit3Btn() { viewModel.DigitInput.OnNext(3); }
        void OnDigit4Btn() { viewModel.DigitInput.OnNext(4); }
        void OnDigit5Btn() { viewModel.DigitInput.OnNext(5); }
        void OnDigit6Btn() { viewModel.DigitInput.OnNext(6); }
        void OnDigit7Btn() { viewModel.DigitInput.OnNext(7); }
        void OnDigit8Btn() { viewModel.DigitInput.OnNext(8); }
        void OnDigit9Btn() { viewModel.DigitInput.OnNext(9); }
        void OnDigit0Btn() { viewModel.DigitInput.OnNext(0); }
        void OnDotBtn()
        {
            viewModel.DotInput.OnNext(Unit.Default);
        }
        void StartProcces()
        {
            cardPaymentPanel.SetActive(true);
            playerCamController.SetLook(playerCamLookTransform.position, playerCamLookTransform.eulerAngles);
        }
        async void OnCompleteProcces()
        {
            cardPaymentPanel.SetActive(false);
            completePaymentPanel.SetActive(true);
            playerCamController.ResetLook();
            await UniTask.WaitForSeconds(completeShowDelay);
            completePaymentPanel.SetActive(false);
        }
        void UpdatePriceText(string text)
        {
            totalPriceTMP.text = text;
        }
        void UpdateEnterText(string text)
        {
            enterAmountTMP.text = text;
        }
    }
}
