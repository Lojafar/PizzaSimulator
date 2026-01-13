using Game.PizzeriaSimulator.BoxCarry.Box.Item;
using Game.PizzeriaSimulator.PizzaCreation;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace Game.PizzeriaSimulator.BoxCarry.Box
{
    class CardboardPizzaIngredientBox : MonoBehaviour, IPizzaIngredientBox
    {
        [field: SerializeField] public PizzaIngredientType IngredientType { get; private set; }
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
        public CarriableBoxType BoxType => CarriableBoxType.PizzaIngredientsBox;
        public int ItemsAmount => items.Count;
        public bool IsOpened { get; private set; }
        private void Awake()
        {
            IsOpened = false;
            openAnim = DOTween.Sequence()
                .Append(firstTopLid.DOLocalRotate(openedLidsRotsInOrder[0], halfOpenDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear))
                .Join(secondTopLid.DOLocalRotate(openedLidsRotsInOrder[1], halfOpenDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear))
                .Append(secondBottomLid.DOLocalRotate(openedLidsRotsInOrder[2], halfOpenDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear))
                .Join(firstBottomLid.DOLocalRotate(openedLidsRotsInOrder[3], halfOpenDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear))
                .SetAutoKill(false);
        }
        public void Open()
        {
            IsOpened = true;
            if (openAnim.IsPlaying()) openAnim.Pause();
            openAnim.PlayForward();
        }
        public void Close()
        {
            IsOpened = false;
            if (openAnim.IsPlaying()) openAnim.Pause();
            openAnim.PlayBackwards();
        }
        public void OnPicked()
        {
            boxRB.isKinematic = true;
            colliderOfBox.enabled = false;
        }
        public void Throw(Vector3 forceVector) 
        {
            boxRB.isKinematic = false;
            colliderOfBox.enabled = true;
            boxRB.AddForce(forceVector, ForceMode.Impulse);
        }
        public BoxItemBase RemoveAndGetItem()
        {
            if (items.Count < 1) return null;
            BoxItemBase boxItem = items[^1];
            items.RemoveAt(items.Count - 1);
            return boxItem;
        }
    }
}
