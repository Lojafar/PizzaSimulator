using DG.Tweening;
using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual.IngredientOnPizza
{
    class SauceOnPizzaObject : IngredientOnPizzaObjectBase
    {
        [SerializeField] float fallDuration;
        [SerializeField] float firstScaleDuration;
        [SerializeField] float secondScaleDuration;
        [SerializeField] Vector3 firstEndScale;
        [SerializeField] Vector3 secondEndScale;
        [SerializeField] GameObject firstSauceObject;
        [SerializeField] GameObject secondSauceObject;
        public override void Place(Vector3 placePosition, bool withAnim = true)
        {
            if (withAnim)
            {
                transform.DOMove(placePosition, fallDuration).OnComplete(OnSauceFalled).Play();
            }
            else
            {
                transform.position = placePosition;
                Destroy(firstSauceObject);
                secondSauceObject.SetActive(true);
                secondSauceObject.transform.localScale = secondEndScale;
            }
        }
        void OnSauceFalled()
        {
            firstSauceObject.transform.DOScale(firstEndScale, firstScaleDuration).OnComplete(OnFirstScaled).Play();
        }
        void OnFirstScaled()
        {
            Destroy(firstSauceObject);
            secondSauceObject.SetActive(true);
            secondSauceObject.transform.DOScale(secondEndScale, secondScaleDuration).Play();
        }
    }
}
