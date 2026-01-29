using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using R3;
using Game.Root.Utils.Audio;
using System.Collections.Generic;
namespace Assets.Game.Scripts.PizzeriaSimulator.UI.NewLevelPopUp.Visual
{
    class NewLevelPopUpView : NewLevelPopUpViewBase
    {
        [SerializeField] AudioClip confettiSFX;
        [SerializeField] AudioClip popUpScaleSFX;
        [SerializeField] RectTransform confettiRTransform;
        [SerializeField] RectTransform popupRTransform;
        [SerializeField] Image bgImage;
        [SerializeField] LevelRewardSlot rewardSlotPrefab;
        [SerializeField] Transform rewardsContainer;
        [SerializeField] Button confirmButton;
        [SerializeField] TMP_Text levelTMP;
        [SerializeField] float bgNormalAlpha;
        [SerializeField] Vector3 endPopupAnchorPos;
        [SerializeField] Vector3 normalPopupAnchorPos;
        [SerializeField] float bgFadeDur;
        [SerializeField] float confettiScaleDur;
        [SerializeField] float popupScaleDelay;
        [SerializeField] float popupScaleDur;
        [SerializeField] float rewardsScaleDelay;
        [SerializeField] float rewardsScaleDur;
        [SerializeField] float confirmBtnScaleDelay;
        [SerializeField] float confirmBtnScaleDur;
        [SerializeField] float popupEndMoveDur;
        readonly List<GameObject> spawnedRewards = new();
        Tween openAnim;
        Tween closeAnim;
        public override void Bind(NewLevelPopUpVM _viewModel)
        {
            confirmButton.onClick.AddListener(OnConfirmBtn);
            base.Bind(_viewModel);
            viewModel.Open += Open;
            viewModel.Close += Close;
            viewModel.ClearAllRewards += ClearAllRewards;
            viewModel.ShowNewReward += SpawnNewReward;
            viewModel.UpdateLevelText += UpdateLevelText;
        }
        private void OnDestroy()
        {
            confirmButton.onClick.RemoveListener(OnConfirmBtn);
            viewModel.Open -= Open;
            viewModel.Close -= Close;
            viewModel.ClearAllRewards -= ClearAllRewards;
            viewModel.ShowNewReward -= SpawnNewReward;
            viewModel.UpdateLevelText -= UpdateLevelText;
        }
        void OnConfirmBtn()
        {
            viewModel.ConfirmInput.OnNext(Unit.Default);
        }
        void Open(bool immediately)
        {
            if (closeAnim != null && closeAnim.IsPlaying()) closeAnim.Pause();
            PlayOpenAnim();
            if (immediately) openAnim.Complete();
        }
        void PlayOpenAnim()
        {
            bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, 0);
            popupRTransform.anchoredPosition = normalPopupAnchorPos;
            popupRTransform.localScale = Vector3.zero;
            confettiRTransform.anchoredPosition = normalPopupAnchorPos;
            confettiRTransform.localScale = Vector3.zero;
            rewardsContainer.localScale = Vector3.zero;
            confirmButton.transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            if(openAnim == null)
            {
                openAnim = DOTween.Sequence()
                .Append(bgImage.DOFade(bgNormalAlpha, bgFadeDur))
                .AppendCallback(() => AudioPlayer.PlaySFX(confettiSFX))
                .Append(confettiRTransform.DOScale(1, confettiScaleDur))
                .AppendInterval(popupScaleDelay)
                .AppendCallback(() => AudioPlayer.PlaySFX(popUpScaleSFX))
                .Append(popupRTransform.DOScale(1, popupScaleDur))
                .AppendInterval(rewardsScaleDelay)
                .AppendCallback(() => AudioPlayer.PlaySFX("UIApear"))
                .Append(rewardsContainer.DOScale(1, rewardsScaleDur))
                .AppendInterval(confirmBtnScaleDelay)
                .AppendCallback(() => AudioPlayer.PlaySFX("UIApear"))
                .Append(confirmButton.transform.DOScale(1, confirmBtnScaleDur))
                .SetAutoKill(false);
            }
            openAnim.Restart();
            openAnim.Play();
        }
        void Close(bool immediately)
        {
            if (openAnim != null && openAnim.IsPlaying()) openAnim.Pause();
            PlayCloseAnim();
            if (immediately) closeAnim.Complete();
        }
        void PlayCloseAnim()
        {
            bgImage.color = new Color(bgImage.color.r, bgImage.color.g, bgImage.color.b, bgNormalAlpha);
            popupRTransform.anchoredPosition = normalPopupAnchorPos;
            popupRTransform.localScale = Vector3.one;
            confettiRTransform.anchoredPosition = normalPopupAnchorPos;
            confettiRTransform.localScale = Vector3.one;
            rewardsContainer.localScale = Vector3.one;
            confirmButton.transform.localScale = Vector3.one;
            if (closeAnim == null) 
            {
                closeAnim = DOTween.Sequence()
                .AppendCallback(() => AudioPlayer.PlaySFX("Swoosh"))
                .Append(confettiRTransform.DOAnchorPos(endPopupAnchorPos, popupEndMoveDur))
                .Join(popupRTransform.DOAnchorPos(endPopupAnchorPos, popupEndMoveDur))
                .Append(bgImage.DOFade(0, bgFadeDur))
                .OnComplete(() => gameObject.SetActive(false))
                .SetAutoKill(false);
            }
            closeAnim.Restart();
            closeAnim.Play();
        }
        void ClearAllRewards()
        {
            for (int i = 0; i < spawnedRewards.Count; i++) 
            {
                Destroy(spawnedRewards[i]);
            }
            spawnedRewards.Clear();
        }
        void SpawnNewReward(Sprite icon, string desription)
        {
            LevelRewardSlot spawnedRewardSlot = Instantiate(rewardSlotPrefab, rewardsContainer);
            spawnedRewardSlot.SetIcon(icon);
            spawnedRewardSlot.SetDescription(desription);
            spawnedRewards.Add(spawnedRewardSlot.gameObject);
        }
        void UpdateLevelText(string text) 
        {
            levelTMP.text = text;
        }
    }
}
