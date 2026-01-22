using UnityEngine;
using DG.Tweening;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual.IngredientOnPizza
{
    class SwapObjectIngredientObject : IngredientOnPizzaObjectBase
    {
        [SerializeField] float fallDuration;
        [SerializeField] GameObject firstObj;
        [SerializeField] GameObject secondObj;
        public override void Place(Vector3 placePosition, bool withAnim = true)
        {
            if (withAnim)
            {
                transform.DOMove(placePosition, fallDuration).OnComplete(OnObjFalled).Play();
            }
            else
            {
                transform.position = placePosition;
                OnObjFalled();
            }
        }
        void OnObjFalled()
        {
            Destroy(firstObj);
            secondObj.SetActive(true);
        }
    }
}
