using DG.Tweening;
using Game.Root.Utils.Audio;
using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Boxes.Item
{
    class JumpBoxItem : BoxItemBase
    {
        [SerializeField] float itemJumpDuration;
        [SerializeField] float itemMoveYPeakAdd;
        [SerializeField] string sfxClipKey;
        public override void SetTo(Vector3 targetPos, Action onCompleted = null)
        {
            AudioPlayer.PlaySFX(sfxClipKey);
            transform.DOJump(targetPos, itemMoveYPeakAdd, 1, itemJumpDuration).SetEase(Ease.Linear).Play();
            onCompleted?.Invoke();
        }
    }
}
