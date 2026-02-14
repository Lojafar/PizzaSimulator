using DG.Tweening;
using UnityEngine;

namespace Game.PizzeriaSimulator.Boxes
{
    abstract class CardboardCarriableBoxBase : CarriableBoxBase
    {
        [SerializeField] protected Rigidbody boxRB;
        [SerializeField] protected Collider colliderOfBox;
        [SerializeField] protected GameObject openVisuals;
        [SerializeField] float halfOpenDuration;
        [SerializeField] Vector3[] openedLidsRotsInOrder = new Vector3[4];
        [SerializeField] Transform firstTopLid;
        [SerializeField] Transform secondTopLid;
        [SerializeField] Transform firstBottomLid;
        [SerializeField] Transform secondBottomLid;
        Tween openAnim;
        protected virtual void Awake()
        {
            boxData.IsOpened = false;
            if (openVisuals != null) openVisuals.SetActive(false);
            openAnim = DOTween.Sequence()
                .AppendCallback(OnCloseAnimEnded)
                .Append(firstTopLid.DOLocalRotate(openedLidsRotsInOrder[0], halfOpenDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear))
                .Join(secondTopLid.DOLocalRotate(openedLidsRotsInOrder[1], halfOpenDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear))
                .Append(firstBottomLid.DOLocalRotate(openedLidsRotsInOrder[2], halfOpenDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear))
                .Join(secondBottomLid.DOLocalRotate(openedLidsRotsInOrder[3], halfOpenDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear))
                .SetAutoKill(false);
        }
        void OnCloseAnimEnded()
        {
            if (!IsOpened && openVisuals != null) openVisuals.SetActive(false);
        }
        public override void SetBoxData(CarriableBoxData _boxData)
        {
            base.SetBoxData(_boxData);
            if (boxData.IsOpened)
            {
                if (openVisuals != null) openVisuals.SetActive(true);
                openAnim.PlayForward();
                openAnim.Complete();
                firstTopLid.localEulerAngles = openedLidsRotsInOrder[0];
                secondTopLid.localEulerAngles = openedLidsRotsInOrder[1];
                firstBottomLid.localEulerAngles = openedLidsRotsInOrder[2];
                secondBottomLid.localEulerAngles = openedLidsRotsInOrder[3];
            }
        }
        public override void Open()
        {
            boxData.IsOpened = true;
            if (openVisuals != null) openVisuals.SetActive(true);
            if (openAnim.IsPlaying()) openAnim.Pause();
            openAnim.PlayForward();
        }
        public override void Close()
        {
            boxData.IsOpened = false;
            if (openAnim.IsPlaying()) openAnim.Pause();
            openAnim.PlayBackwards();
        }
        public override void OnPicked()
        {
            boxRB.isKinematic = true;
            colliderOfBox.enabled = false;
        }
        public override void Throw(Vector3 forceVector)
        {
            boxRB.isKinematic = false;
            colliderOfBox.enabled = true;
            boxRB.AddForce(forceVector, ForceMode.Impulse);
        }
    }
}
