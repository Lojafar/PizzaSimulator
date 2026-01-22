using Game.PizzeriaSimulator.Boxes.Item;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
namespace Game.PizzeriaSimulator.Boxes
{
    class CardboardPizzaIngredientBox : PizzaIngredientBoxBase
    {
        [SerializeField] List<BoxItemBase> items;
        [SerializeField] Rigidbody boxRB;
        [SerializeField] Collider colliderOfBox;
        [SerializeField] float halfOpenDuration;
        [SerializeField] Vector3[] openedLidsRotsInOrder = new Vector3[4];
        [SerializeField] Transform firstTopLid;
        [SerializeField] Transform secondTopLid;
        [SerializeField] Transform firstBottomLid;
        [SerializeField] Transform secondBottomLid;
        Tween openAnim;
        public override CarriableBoxType BoxType => CarriableBoxType.PizzaIngredientsBox;
        private void Awake()
        {
            boxData.ItemsAmount = items.Count;
            boxData.IsOpened = false;
            openAnim = DOTween.Sequence()
                .Append(firstTopLid.DOLocalRotate(openedLidsRotsInOrder[0], halfOpenDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear))
                .Join(secondTopLid.DOLocalRotate(openedLidsRotsInOrder[1], halfOpenDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear))
                .Append(firstBottomLid.DOLocalRotate(openedLidsRotsInOrder[2], halfOpenDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear))
                .Join(secondBottomLid.DOLocalRotate(openedLidsRotsInOrder[3], halfOpenDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear))
                .SetAutoKill(false);
        }
        public override void SetBoxData(CarriableBoxData _boxData)
        {
            base.SetBoxData(_boxData);
            if (boxData.IsOpened)
            {
                openAnim.PlayForward();
                openAnim.Complete();
                firstTopLid.localEulerAngles = openedLidsRotsInOrder[0];
                secondTopLid.localEulerAngles = openedLidsRotsInOrder[1];
                firstBottomLid.localEulerAngles = openedLidsRotsInOrder[2];
                secondBottomLid.localEulerAngles = openedLidsRotsInOrder[3];
            }
            for (int itemsToRemove = items.Count - boxData.ItemsAmount; itemsToRemove > 0; itemsToRemove--)
            {
                Destroy(items[^1].gameObject);
                items.RemoveAt(items.Count - 1);
            }
        }
        public override void Open()
        {
            boxData.IsOpened = true;
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
        public override BoxItemBase RemoveAndGetItem()
        {
            if (items.Count < 1) return null;
            BoxItemBase boxItem = items[^1];
            items.RemoveAt(items.Count - 1);
            boxData.ItemsAmount = items.Count;
            return boxItem;
        }
    }
}
