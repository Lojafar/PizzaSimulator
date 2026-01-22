using DG.Tweening;
using UnityEngine;
namespace Game.PizzeriaSimulator.PizzaCreation.Visual.IngredientOnPizza
{
     class DoughOnPizzaObject : IngredientOnPizzaObjectBase
     {
        [SerializeField] float firstDoughAnimDuration;
        [SerializeField] float secondDoughAnimDuration;
        [SerializeField] Vector3 firstDoughFlatScale;
        [SerializeField] Vector3 secondDoughFlatScale;
        [SerializeField] GameObject firstDough;
        [SerializeField] GameObject secondDough;
        public override void Place(Vector3 placePosition, bool withAnim = true)
        {
            if (withAnim)
            {
            DOTween.Sequence().Join(transform.DOMove(placePosition, firstDoughAnimDuration)).Join(firstDough.transform.DOScale(firstDoughFlatScale, firstDoughAnimDuration))
                .OnComplete(OnFirstDoughFalled).Play();
            }
            else
            {
                transform.position = placePosition;
                Destroy(firstDough);
                secondDough.SetActive(true);
                secondDough.transform.localScale = secondDoughFlatScale;
            }
        }
        void OnFirstDoughFalled()
        {
            Destroy(firstDough);
            secondDough.SetActive(true);
            secondDough.transform.DOScale(secondDoughFlatScale, secondDoughAnimDuration).Play();
        }
    }
}
