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
        [SerializeField] Material bakedMaterial;
        [SerializeField] MeshRenderer meshRenderer;
        public override void DoPlaceAnim(Vector3 placePosition)
        {
            DOTween.Sequence().Join(transform.DOMove(placePosition, firstDoughAnimDuration)).Join(firstDough.transform.DOScale(firstDoughFlatScale, firstDoughAnimDuration))
                .OnComplete(OnFirstDoughFalled).Play();
        }
        void OnFirstDoughFalled()
        {
            Destroy(firstDough);
            secondDough.SetActive(true);
            secondDough.transform.DOScale(secondDoughFlatScale, secondDoughAnimDuration).Play();
        }
    }
}
