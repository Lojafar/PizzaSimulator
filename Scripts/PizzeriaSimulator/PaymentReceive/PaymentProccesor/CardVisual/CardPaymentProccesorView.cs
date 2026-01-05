using Game.PizzeriaSimulator.Player.CameraController;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System;
using Game.Root.Utils.Audio;
using Game.Root.Utils;

namespace Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor.Visual
{
    class CardPaymentProccesorView : CardPaymentProccesorViewBase
    {
        [SerializeField] AudioClip btnClickSFX;
        [SerializeField] AudioClip paySuccesSFX;
        [SerializeField] AudioClip errorSFX;
        [SerializeField] Transform inputCamLookTransform;
        [SerializeField] Transform completedCamLookTransform;
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
            viewModel.ShowErrorMessage += ShowErrorMessage;
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
                viewModel.ShowErrorMessage -= ShowErrorMessage;
            }
        }
        void OnConfirmBtn()
        {
            viewModel.ConfirmInput.OnNext(Unit.Default);
        }
        void OnBackBtn()
        {
            OnInputBtn();
            viewModel.BackInput.OnNext(Unit.Default);
        }
        void OnDigit1Btn() { OnInputBtn(); viewModel.DigitInput.OnNext(1); }
        void OnDigit2Btn() { OnInputBtn(); viewModel.DigitInput.OnNext(2); }
        void OnDigit3Btn() { OnInputBtn(); viewModel.DigitInput.OnNext(3); }
        void OnDigit4Btn() { OnInputBtn(); viewModel.DigitInput.OnNext(4); }
        void OnDigit5Btn() { OnInputBtn(); viewModel.DigitInput.OnNext(5); }
        void OnDigit6Btn() { OnInputBtn(); viewModel.DigitInput.OnNext(6); }
        void OnDigit7Btn() { OnInputBtn(); viewModel.DigitInput.OnNext(7); }
        void OnDigit8Btn() { OnInputBtn(); viewModel.DigitInput.OnNext(8); }
        void OnDigit9Btn() { OnInputBtn(); viewModel.DigitInput.OnNext(9); }
        void OnDigit0Btn() { OnInputBtn(); viewModel.DigitInput.OnNext(0); }
        void OnDotBtn()
        {
            OnInputBtn();
            viewModel.DotInput.OnNext(Unit.Default);
        }
        void OnInputBtn()
        {
            AudioPlayer.PlaySFX(btnClickSFX);
        }
        void StartProcces()
        {
            cardPaymentPanel.SetActive(true);
            playerCamController.SetLook(inputCamLookTransform.position, inputCamLookTransform.eulerAngles);
        }
        async void OnCompleteProcces(Action onShowed)
        {
            cardPaymentPanel.SetActive(false);
            completePaymentPanel.SetActive(true);
            playerCamController.SetLook(completedCamLookTransform.position, completedCamLookTransform.eulerAngles);
            AudioPlayer.PlaySFX(paySuccesSFX);
            await UniTask.WaitForSeconds(completeShowDelay);
            completePaymentPanel.SetActive(false);
            playerCamController.ResetLook();
            onShowed?.Invoke();
        }
        void UpdatePriceText(string text)
        {
            totalPriceTMP.text = text;
        }
        void UpdateEnterText(string text)
        {
            enterAmountTMP.text = text;
        }
        void ShowErrorMessage(string text) 
        {
            Toasts.ShowToast(text);
            AudioPlayer.PlaySFX(errorSFX);
        }
    }
}
