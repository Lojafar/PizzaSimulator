using DG.Tweening;
using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.BoxCarry.Box.Item
{
    class SauceCanBoxItem : BoxItemBase
    {
        [SerializeField] float moveDuration;
        [SerializeField] float rotationDuration;
        [SerializeField] float fluidDuration;
        [SerializeField] float canYOffset;
        [SerializeField] Vector3 openedRotation;
        [SerializeField] GameObject fluidObject;
        public override void SetTo(Vector3 targetPos, Action onCompleted = null)
        {
            transform.parent = null;
            DOTween.Sequence()
                .Append(transform.DOMove(new Vector3(targetPos.x, targetPos.y + canYOffset, targetPos.z), moveDuration).SetEase(Ease.Linear))
                .Append(transform.DORotate(openedRotation, rotationDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear)).Play()
                .AppendCallback(() => {fluidObject.SetActive(true); onCompleted?.Invoke(); })
                .AppendInterval(fluidDuration)
                .AppendCallback(() => { gameObject.SetActive(false);  }).Play();
        }
    }
}
