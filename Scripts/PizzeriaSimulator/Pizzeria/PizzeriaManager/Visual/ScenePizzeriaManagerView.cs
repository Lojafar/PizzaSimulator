using DG.Tweening;
using Game.Root.Utils.Audio;
using UnityEngine;
namespace Game.PizzeriaSimulator.Pizzeria.Manager.Visual
{
    class ScenePizzeriaManagerView : PizzeriaManagerViewBase
    {
        [SerializeField] AudioClip switchSignSFX;
        [SerializeField] Transform openClosedSign;
        [SerializeField] Vector3 openSignLocRot;
        [SerializeField] Vector3 closedSignLocRot;
        [SerializeField] Vector3 preRotSignLocPos;
        Vector3 normalSignLocPos;
        [SerializeField] float signRotDuration;
        [SerializeField] float signMoveDuration;
        Tween openSignTween;
        public override void Bind(PizzeriaManagerVM _managerVM)
        {
            base.Bind(_managerVM);
            managerVM.OpenPizzeria += OnPizzeriaOpen;
            managerVM.ClosePizzeria += OnPizzeriaClose;

            normalSignLocPos = openClosedSign.localPosition;
            openClosedSign.localEulerAngles = closedSignLocRot;
            openSignTween = DOTween.Sequence()
                .Append(openClosedSign.DOLocalMove(preRotSignLocPos, signMoveDuration).SetEase(Ease.Linear))
                .Append(openClosedSign.DOLocalRotate(openSignLocRot, signRotDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear))
                .Append(openClosedSign.DOLocalMove(normalSignLocPos, signMoveDuration).SetEase(Ease.Linear)).SetAutoKill(false);
        }
        void OnPizzeriaOpen(bool immediately)
        {
            if (openSignTween.IsPlaying()) openSignTween.Pause();

            openSignTween.Restart();
            openSignTween.PlayForward();
            if (immediately)
            {
                openSignTween.Complete();
            }
            else
            {
                AudioPlayer.PlaySFX(switchSignSFX);
            }
        }
        void OnPizzeriaClose(bool immediately)
        {
            if (openSignTween.IsPlaying()) openSignTween.Pause();

            openSignTween.PlayForward();
            openSignTween.Complete();
            openSignTween.PlayBackwards();
            if (immediately)
            {
                openSignTween.Complete();
            }
            else
            {
                AudioPlayer.PlaySFX(switchSignSFX);
            }
        }
    }
}
