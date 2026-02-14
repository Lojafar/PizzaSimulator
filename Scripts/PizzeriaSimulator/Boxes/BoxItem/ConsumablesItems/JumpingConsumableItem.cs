using DG.Tweening;
using Game.Root.Utils.Audio;
using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Boxes.Item.Consumable
{
    sealed class JumpingConsumableItem : ConsumableBoxItemBase
    {
        [SerializeField] float itemJumpDuration;
        [SerializeField] float itemMoveYPeakAdd;
        public override void SetTo(Vector3 targetPos, Vector3 targetRot, Action onCompleted = null)
        {
            AudioPlayer.PlaySFX("ItemSwoosh");
            DOTween.Sequence()
                .Append(transform.DOJump(targetPos, itemMoveYPeakAdd, 1, itemJumpDuration).SetEase(Ease.Linear))
                .Join(transform.DORotate(targetRot, itemJumpDuration).SetEase(Ease.Linear))
                .AppendCallback(() => onCompleted?.Invoke())
                .Play();
        }
    }
}
