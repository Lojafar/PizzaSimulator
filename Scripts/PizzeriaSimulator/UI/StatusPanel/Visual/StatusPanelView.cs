using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
namespace Game.PizzeriaSimulator.UI.StatusPanel.Visual
{
    class StatusPanelView : StatusPanelViewBase
    {
        [SerializeField] TMP_Text moneyAmountTMP;
        [SerializeField] TMP_Text gemsAmountTMP;
        [SerializeField] TMP_Text timeTMP;
        [SerializeField] TMP_Text levelTMP;
        [SerializeField] Slider lvlProgressSlider;
        [SerializeField] Transform lvlProgressParent;
        [SerializeField] float progressScaleDur;
        [SerializeField] float progressFillDelay;
        [SerializeField] float progressFillDur;
        [SerializeField] Vector3 lvlProgressBigScale;
        bool progressFillWasUped;
        Tween lvlProgressScaleAnim;
        Tween curProgressFillTween;

        public override void Bind(StatusPanelVM _viewModel)
        {
            base.Bind(_viewModel);
            viewModel.UpdateMoneyText += UpdateMoneyText;
            viewModel.UpdateGemsText += UpdateGemsText;
            viewModel.UpdateTimeText += UpdateTimeText;
            viewModel.UpdateLevelText += UpdateLvlText;
            viewModel.UpdateLvlProgress += UpdateLvlFill;
            lvlProgressScaleAnim = DOTween.Sequence()
                .Append(lvlProgressParent.DOScale(lvlProgressBigScale, progressScaleDur).SetEase(Ease.Linear))
                .AppendInterval(progressFillDelay * 2 + progressFillDur)
                .Append(lvlProgressParent.DOScale(1, progressScaleDur).SetEase(Ease.Linear))
                .SetAutoKill(false);
        }
        private void OnDestroy()
        {
            viewModel.UpdateMoneyText -= UpdateMoneyText;
            viewModel.UpdateGemsText -= UpdateGemsText;
            viewModel.UpdateTimeText -= UpdateTimeText;
            viewModel.UpdateLevelText -= UpdateLvlText;
            viewModel.UpdateLvlProgress -= UpdateLvlFill;
        }
        void UpdateMoneyText(string moneyText)
        {
            moneyAmountTMP.text = moneyText;
        }
        void UpdateGemsText(string gemsText) 
        {
            gemsAmountTMP.text = gemsText;
        }
        void UpdateTimeText(string timeText) 
        {
            timeTMP.text = timeText;
        }
        void UpdateLvlText(string timeText)
        {
            levelTMP.text = timeText;
        }
        void UpdateLvlFill(float progress)
        {
            if (!progressFillWasUped)
            {
                lvlProgressSlider.value = progress;
                progressFillWasUped = true;
                return;
            }
            if (lvlProgressScaleAnim.IsPlaying()) lvlProgressScaleAnim.Pause();
            if (curProgressFillTween != null && curProgressFillTween.IsActive()) curProgressFillTween.Kill();
            lvlProgressScaleAnim.Restart();
            lvlProgressScaleAnim.Play();
            curProgressFillTween = DOTween.Sequence()
                .AppendInterval(progressScaleDur + progressFillDelay) 
                .Append(lvlProgressSlider.DOValue(progress, progressFillDur).SetEase(Ease.Linear))
                .Play();
        }
    }
}