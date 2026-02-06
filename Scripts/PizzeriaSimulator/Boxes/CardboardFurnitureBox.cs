using Game.PizzeriaSimulator.Boxes.Item;
using UnityEngine;
using DG.Tweening;
namespace Game.PizzeriaSimulator.Boxes
{
    sealed class CardboardFurnitureBox : FurnitureBoxBase
    {
        [SerializeField] FurnitureBoxItemBase furnitureItem;
        [SerializeField] Rigidbody boxRB;
        [SerializeField] Collider colliderOfBox;
        [SerializeField] GameObject openVisuals;
        [SerializeField] float halfOpenDuration;
        [SerializeField] Vector3[] openedLidsRotsInOrder = new Vector3[4];
        [SerializeField] Transform firstTopLid;
        [SerializeField] Transform secondTopLid;
        [SerializeField] Transform firstBottomLid;
        [SerializeField] Transform secondBottomLid;
        Tween openAnim;
        public override CarriableBoxType BoxType => CarriableBoxType.FurnitureBox;
        private void Awake()
        {
            if (furnitureItem != null)
            {
                furnitureItem.transform.parent = null;
                furnitureItem.gameObject.SetActive(false);
            }
            boxData.ItemsAmount = 1;
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
            boxData.ItemsAmount = _boxData.ItemsAmount;
            if (ItemsAmount == 0 ) 
            {
                if (furnitureItem != null) Destroy(furnitureItem.gameObject);
                if (openVisuals != null) Destroy(openVisuals);
            }
        }
        public override void Open()
        {
            boxData.IsOpened = true;
            if (openVisuals != null) openVisuals.SetActive(true);
            if (openAnim.IsPlaying()) openAnim.Pause();
            openAnim.PlayForward();
            if (furnitureItem != null) furnitureItem.gameObject.SetActive(true);
        }
        public override void Close()
        {
            boxData.IsOpened = false;
            if (openAnim.IsPlaying()) openAnim.Pause();
            openAnim.PlayBackwards();
            if (furnitureItem != null) furnitureItem.gameObject.SetActive(false);
        }
        public override void OnPicked()
        {
            if (IsOpened && furnitureItem != null)
            {
                furnitureItem.gameObject.SetActive(true);
            }
            boxRB.isKinematic = true;
            colliderOfBox.enabled = false;
        }
        public override void Throw(Vector3 forceVector)
        {
            if (furnitureItem != null)
            {
                furnitureItem.gameObject.SetActive(false);
            }
            boxRB.isKinematic = false;
            colliderOfBox.enabled = true;
            boxRB.AddForce(forceVector, ForceMode.Impulse);
        }
        public override FurnitureBoxItemBase GetFurnitureItem()
        {
            return furnitureItem;
        }
        public override void RemoveItem()
        {
            boxData.ItemsAmount--;
            if(furnitureItem != null ) Destroy(furnitureItem.gameObject);
            if(openVisuals != null ) Destroy(openVisuals);
        }
    }
}
