using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.Root.Utils.Toast
{
    class ToastObject : ToastObjectBase
    {
        [SerializeField] RectTransform rectTransform;
        [SerializeField] TMP_Text textTMP;
        [SerializeField] Vector3 toastShowPos;
        [SerializeField] Vector3 toastHidePos;
        [SerializeField] float moveDuration;
        [SerializeField] float showDuration;
        private void Awake()
        {
            rectTransform.anchoredPosition = toastHidePos;
        }
        public override void Show(string text)
        {
            textTMP.text = text;
            DOTween.Sequence().Append(rectTransform.DOLocalMove(toastShowPos, moveDuration)).
                Append(rectTransform.DOLocalMove(toastShowPos, showDuration)).
                Append(rectTransform.DOLocalMove(toastHidePos, moveDuration)).Play();
        }
    }
}
