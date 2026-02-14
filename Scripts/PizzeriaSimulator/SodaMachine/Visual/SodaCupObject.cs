using DG.Tweening;
using UnityEngine;

namespace Game.PizzeriaSimulator.SodaMachine.Visual
{
    class SodaCupObject : MonoBehaviour
    {
        [SerializeField] Transform sodaInCupTransform;
        [SerializeField] Transform sodaCapTransform;
        [SerializeField] Vector3 sodaCapEndScale;
        [SerializeField] float capZeroScaleLPosY;
        [SerializeField] float fullSodaLocalPosY;
        [SerializeField] float sodaCapScaleDur;
        float capNormScaleLPosY;
        bool filled;
        private void Awake()
        {
            sodaInCupTransform.gameObject.SetActive(false);
            sodaCapTransform.gameObject.SetActive(false);
        }
        public void FillCup(float duration)
        {
            if (filled) return;
            filled = true;
            capNormScaleLPosY = sodaCapTransform.localPosition.y;
            sodaCapTransform.localPosition = Vector3.up * capZeroScaleLPosY;
            sodaCapTransform.localScale = Vector3.zero;
            sodaInCupTransform.gameObject.SetActive(true);
            sodaCapTransform.gameObject.SetActive(true);
            if(Mathf.Approximately(duration, 0)) sodaCapScaleDur = 0;
            DOTween.Sequence()
                .Append(sodaInCupTransform.DOLocalMoveY(fullSodaLocalPosY, duration).SetEase(Ease.Linear))
                .Append(sodaCapTransform.DOLocalMoveY(capNormScaleLPosY, sodaCapScaleDur).SetEase(Ease.Linear))
                .Join(sodaCapTransform.DOScale(sodaCapEndScale, sodaCapScaleDur).SetEase(Ease.Linear))
                .Play();
        }
    }
}
