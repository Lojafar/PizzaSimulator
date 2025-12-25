using UnityEngine;
using DG.Tweening;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual.IngredientOnPizza
{
    class SwapObjectIngredientObject : IngredientOnPizzaObjectBase
    {
        [SerializeField] float fallDuration;
        [SerializeField] GameObject firstObj;
        [SerializeField] GameObject secondObj;
        [SerializeField] GameObject bakedObj;
        public override void DoPlaceAnim(Vector3 placePosition)
        {
            transform.DOMove(placePosition, fallDuration).OnComplete(OnObjFalled).Play();
        }
        void OnObjFalled()
        {
            Destroy(firstObj);
            secondObj.SetActive(true);
        }
    }
}
