using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Helps;
using Game.PizzeriaSimulator.Player.CameraController;
using Game.Root.Utils;
using Game.Root.Utils.Audio;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor.Visual
{
    using Random = UnityEngine.Random;
    class CashPaymentProccesorView : CashPaymentProccesorViewBase
    {
        [SerializeField] AudioClip openSFX;
        [SerializeField] AudioClip closeSFX;
        [SerializeField] AudioClip banknoteSFX;
        [SerializeField] AudioClip coinSFX;
        [SerializeField] AudioClip errorSFX;
        [SerializeField] Transform shelveTransform;
        [SerializeField] Transform playerCamLookTransform;
        [SerializeField] GameObject cashPaymentPanel;
        [SerializeField] GameObject completePaymentPanel;
        [SerializeField] Button confirmButton;
        [SerializeField] Button backButton;
        [SerializeField] Button clearButton;
        [SerializeField] TMP_Text priceText;
        [SerializeField] TMP_Text giveText;
        [SerializeField] TMP_Text changeText;
        [SerializeField] TMP_Text givedChangeText;
        [SerializeField] Color targetChangeColor = Color.green;
        [SerializeField] Color notTargetChangeColor = Color.red;
        [SerializeField] Vector3 openedShelvePos;
        [SerializeField] Vector3 minChangePos;
        [SerializeField] Vector3 maxChangePos;
        [SerializeField] Vector3 firstPosOfChangeMove;
        [SerializeField] float shelveMoveDuration;
        [SerializeField] float completeShowDelay;
        [SerializeField] float changeMoveDuration;

        DiContainer diContainer;
        PlayerCameraControllerBase playerCamController;
        Camera mainCam;
        CashChangeObject lastSelectedCash;
        RaycastHit raycastHit;
        Stack<GameObject> spawnedChangeObjs;
        int raycastLayerMask;

        Tween shelveOpenTween;
        const float maxCashSelectRayDis = 3;
        [Inject]
        public void Construct(DiContainer _diContainer)
        {
            diContainer = _diContainer;
        }
        public override void Bind(CashPaymentProccesorVM _viewModel)
        {
            spawnedChangeObjs = new Stack<GameObject>();
            raycastLayerMask = LayerMask.GetMask(Layers.DefaultLayerName);
            playerCamController = diContainer.Resolve<PlayerCameraControllerBase>();
            mainCam = Camera.main;
            viewModel = _viewModel;
            confirmButton.onClick.AddListener(OnConfirmBtn);
            backButton.onClick.AddListener(OnBackBtn);
            clearButton.onClick.AddListener(OnClearBtn);
            viewModel.OnStartProcces += StartProcces;
            viewModel.OnCompleteProcces += OnCompleteProcces;
            viewModel.UpdatePriceText += UpdatePriceText;
            viewModel.UpdateGiveText += UpdateGiveText;
            viewModel.UpdateChangeText += UpdateChangeText;
            viewModel.UpdateGivedChangeText += UpdateGivedChangeText;
            viewModel.SetGivedChangeIsTarget += UpdateGivedChangeColor;
            viewModel.OnConfirmedActiveCash += SpawnActiveCashObj;
            viewModel.OnCancelledLastCash += DestroyLastCashObj;
            viewModel.OnClearedAllCash += DestroyAllCashObjs;
            viewModel.ShowErrorMessage += ShowErrorMessage;

            shelveOpenTween = shelveTransform.DOLocalMove(openedShelvePos, shelveMoveDuration).SetAutoKill(false);
        }
        private void OnDestroy()
        {
            confirmButton.onClick.RemoveListener(OnConfirmBtn);
            backButton.onClick.RemoveListener(OnBackBtn);
            clearButton.onClick.RemoveListener(OnClearBtn);
            if (viewModel != null)
            {
                viewModel.OnStartProcces -= StartProcces;
                viewModel.OnCompleteProcces -= OnCompleteProcces;
                viewModel.UpdatePriceText -= UpdatePriceText;
                viewModel.UpdateGiveText -= UpdateGiveText;
                viewModel.UpdateChangeText -= UpdateChangeText;
                viewModel.UpdateGivedChangeText -= UpdateGivedChangeText;
                viewModel.SetGivedChangeIsTarget -= UpdateGivedChangeColor;
                viewModel.OnConfirmedActiveCash -= SpawnActiveCashObj;
                viewModel.OnCancelledLastCash -= DestroyLastCashObj;
                viewModel.OnClearedAllCash -= DestroyAllCashObjs;
                viewModel.ShowErrorMessage -= ShowErrorMessage;
            }
            Ticks.Instance.OnFixedTick -= OnFixedUpdateWhenProcces;
            Ticks.Instance.OnTick -= OnUpdateWhenProcces;
        }
        void OnConfirmBtn()
        {
            viewModel.ConfirmInput.OnNext(Unit.Default);
        }
        void OnBackBtn()
        {
            viewModel.BackInput.OnNext(Unit.Default);
        }
        void OnClearBtn()
        {
            viewModel.ClearInput.OnNext(Unit.Default);
        }
        void StartProcces()
        {
            cashPaymentPanel.SetActive(true);
            shelveOpenTween.Restart();
            shelveOpenTween.Play();
            playerCamController.SetLook(playerCamLookTransform.position, playerCamLookTransform.eulerAngles);
            Ticks.Instance.OnFixedTick += OnFixedUpdateWhenProcces;
            Ticks.Instance.OnTick += OnUpdateWhenProcces;
            AudioPlayer.PlaySFX(openSFX);
        }
        void OnUpdateWhenProcces()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (lastSelectedCash == null) return;
                viewModel.ChangeInput.OnNext(lastSelectedCash.Amount);
            }
        }
        void OnFixedUpdateWhenProcces()
        {
            if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out raycastHit, maxCashSelectRayDis, raycastLayerMask)
                && raycastHit.collider.TryGetComponent<CashChangeObject>(out CashChangeObject cashChangeObject))
            {
                if (lastSelectedCash != cashChangeObject)
                {
                    if (lastSelectedCash != null) lastSelectedCash.Deselect();
                    lastSelectedCash = cashChangeObject;
                    cashChangeObject.Select();
                    
                }
            }
            else if (lastSelectedCash != null)
            {
                lastSelectedCash.Deselect();
                lastSelectedCash = null;
            }
        }
        async void OnCompleteProcces(Action onAnimShowed)
        {
            DestroyAllCashObjs();
            cashPaymentPanel.SetActive(false);
            completePaymentPanel.SetActive(true);
            Ticks.Instance.OnFixedTick -= OnFixedUpdateWhenProcces;
            Ticks.Instance.OnTick -= OnUpdateWhenProcces;
            AudioPlayer.PlaySFX(closeSFX);
            await UniTask.WaitForSeconds(completeShowDelay);
            shelveOpenTween.PlayBackwards();
            completePaymentPanel.SetActive(false);
            playerCamController.ResetLook();
            onAnimShowed?.Invoke();
        }
        void UpdatePriceText(string newText)
        {
            priceText.text = newText;
        }
        void UpdateGiveText(string newText)
        {
            giveText.text = newText;
        }
        void UpdateChangeText(string newText)
        {
            changeText.text = newText;
        }
        void UpdateGivedChangeText(string newText)
        {
            givedChangeText.text = newText;
        }
        void UpdateGivedChangeColor(bool isTarget)
        {
            givedChangeText.color = isTarget ? targetChangeColor : notTargetChangeColor;
        }
        void SpawnActiveCashObj()
        {
            if (lastSelectedCash == null) return;
            GameObject spawnedChangeObj = Instantiate(lastSelectedCash.SinglePrefab, lastSelectedCash.transform.position + new Vector3(0, 0.2f, 0), lastSelectedCash.SinglePrefab.transform.rotation);
            DOTween.Sequence()
                .Append(spawnedChangeObj.transform.DOMove(firstPosOfChangeMove, changeMoveDuration / 2))
                .Append(spawnedChangeObj.transform.DOMove(new Vector3(Random.Range(minChangePos.x, maxChangePos.x), minChangePos.y + 0.0001f * spawnedChangeObjs.Count, Random.Range(minChangePos.z, maxChangePos.z)), changeMoveDuration / 2)).Play();
            spawnedChangeObjs.Push(spawnedChangeObj);
            AudioPlayer.PlaySFX(lastSelectedCash.IsBanknote ? banknoteSFX : coinSFX);
        }
        void DestroyLastCashObj()
        {
            if (spawnedChangeObjs.Count < 1) return;
            Destroy(spawnedChangeObjs.Pop());
        }
        void DestroyAllCashObjs()
        {
            while(spawnedChangeObjs.Count > 0)
            {
                Destroy(spawnedChangeObjs.Pop());
            }
        }
        void ShowErrorMessage(string message)
        {
            Toasts.ShowToast(message);
            AudioPlayer.PlaySFX(errorSFX);
        }

    }
}
