using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Game.Root.Utils.Audio;
namespace Game.PizzeriaSimulator.UI.EndDayStatsPanel.Visual
{
    class EndDayStatsView : EndDayStatsViewBase
    {
        [SerializeField] RectTransform receiptRTransform;
        [SerializeField] Button confirmButton;
        [SerializeField] Image bgImage;
        [SerializeField] TMP_Text customersTMP;
        [SerializeField] TMP_Text unitsSoldTMP;
        [SerializeField] TMP_Text purchasesTMP;
        [SerializeField] TMP_Text salesTMP;
        [SerializeField] TMP_Text tipsTMP;
        [SerializeField] TMP_Text profitTMP;
        [SerializeField] Color negativeColor = Color.red;
        [SerializeField] Color positiveColor = Color.green;
        [SerializeField] Vector3 receiptNormalAnchPos;
        [SerializeField] Vector3 receiptStartAnchPos;
        [SerializeField] Vector3 receiptEndAnchPos;
        [SerializeField] float confirmBtnScaleDur;
        [SerializeField] float confirmBtnScaleDelay;
        [SerializeField] float receiptMoveDur;
        [SerializeField] float bgFadeDur;
        [SerializeField] Ease confirmBtnScaleEase;
        [SerializeField] Ease receiptMoveEase;
        [SerializeField] Ease bgFadeEase;
        Tween openAnim;
        Tween closeAnim;
        public override void Bind(EndDayStatsVM _viewModel)
        {
            confirmButton.onClick.AddListener(OnConfirmBtn);
            base.Bind(_viewModel);
            viewModel.OnOpen += Open;
            viewModel.OnClose += Close;
            viewModel.UpdateCustomersText += UpdateCustomersText;
            viewModel.UpdateUnitsText += UpdateUnitsText;
            viewModel.UpdatePurchasesText += UpdatePurchases;
            viewModel.UpdateSalesText += UpdateSalesText;
            viewModel.UpdateTipsText += UpdateTipsText;
            viewModel.UpdateTotalProfitText += UpdateProfitText;
        }
        private void OnDestroy()
        {
            confirmButton.onClick.RemoveListener(OnConfirmBtn);
            viewModel.OnOpen -= Open;
            viewModel.OnClose -= Close;
            viewModel.UpdateCustomersText -= UpdateCustomersText;
            viewModel.UpdateUnitsText -= UpdateUnitsText;
            viewModel.UpdatePurchasesText -= UpdatePurchases;
            viewModel.UpdateSalesText -= UpdateSalesText;
            viewModel.UpdateTipsText -= UpdateTipsText;
            viewModel.UpdateTotalProfitText -= UpdateProfitText;
        }
        void OnConfirmBtn()
        {
             viewModel.ConfirmInput();
        }
        void Open(bool immediately)
        {
            if (closeAnim != null && closeAnim.IsPlaying()) closeAnim.Pause();
            PlayOpenAnim();
            if (immediately)
                openAnim.Complete();
        }
        void PlayOpenAnim()
        {
            bgImage.color = Color.white;
            confirmButton.transform.localScale = Vector3.zero;
            receiptRTransform.anchoredPosition = receiptStartAnchPos;
            gameObject.SetActive(true);
            if(openAnim == null)
            {
                openAnim = DOTween.Sequence()
                .AppendCallback(() => AudioPlayer.PlaySFX("Swoosh"))
                .Append(receiptRTransform.DOAnchorPos(receiptNormalAnchPos, receiptMoveDur).SetEase(receiptMoveEase))
                .AppendInterval(confirmBtnScaleDelay)
                .AppendCallback(() => AudioPlayer.PlaySFX("UIApear"))
                .Append(confirmButton.transform.DOScale(Vector3.one, confirmBtnScaleDur).SetEase(confirmBtnScaleEase)).SetAutoKill(false);
            }
            openAnim.Restart();
            openAnim.Play();
        }
        void Close(bool immediately)
        {
            if (openAnim != null && openAnim.IsPlaying()) openAnim.Pause();
            PlayCloseAnim();
            if (immediately)
            closeAnim.Complete();
        }
        void PlayCloseAnim()
        {
            bgImage.color = Color.white;
            confirmButton.transform.localScale = Vector3.one;
            receiptRTransform.anchoredPosition = receiptNormalAnchPos;
            if(closeAnim == null)
            {
                closeAnim = DOTween.Sequence()
                .AppendCallback(() => AudioPlayer.PlaySFX("UIDisapear"))
                .Append(confirmButton.transform.DOScale(Vector3.zero, confirmBtnScaleDur).SetEase(confirmBtnScaleEase))
                .AppendInterval(confirmBtnScaleDelay)
                .AppendCallback(() => AudioPlayer.PlaySFX("Swoosh"))
                .Append(receiptRTransform.DOAnchorPos(receiptEndAnchPos, receiptMoveDur).SetEase(receiptMoveEase))
                .Append(bgImage.DOFade(0, bgFadeDur).SetEase(bgFadeEase))
                .OnComplete(() => gameObject.SetActive(false))
                .SetAutoKill(false);
            }
            closeAnim.Restart();
            closeAnim.Play();
        }
        void UpdateCustomersText(string text) 
        {
            customersTMP.text = text;
        }
        void UpdateUnitsText(string text) 
        {
            unitsSoldTMP.text = text;
        }
        void UpdatePurchases(string text, bool positive)
        {
            purchasesTMP.text = text;
            purchasesTMP.color = positive ? positiveColor : negativeColor;
        }
        void UpdateSalesText(string text, bool positive)
        {
            salesTMP.text = text;
            salesTMP.color = positive ? positiveColor : negativeColor;
        }
        void UpdateTipsText(string text, bool positive)
        {
            tipsTMP.text = text;
            tipsTMP.color = positive ? positiveColor : negativeColor;
        }
        void UpdateProfitText(string text, bool positive)
        {
            profitTMP.text = text;
            profitTMP.color = positive ? positiveColor : negativeColor;
        }
    }
}
