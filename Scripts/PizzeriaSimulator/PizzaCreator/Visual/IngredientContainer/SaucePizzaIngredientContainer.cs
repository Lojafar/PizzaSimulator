using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual
{
    class SaucePizzaIngredientContainer : PizzaIngredientContainerBase
    {
        [SerializeField] Transform sauceTransform;
        [SerializeField] Transform spoonTransform;
        [SerializeField] GameObject sauceInSpoonObject;
        [SerializeField] float oneItemScaleChange;
        [SerializeField] Vector3 normalSpoonRot;
        [SerializeField] Vector3 normalSpoonPos;
        [SerializeField] Vector3 dragSpoonRot;

        bool isIngredientDragging;

        public override Vector3 GetDragIngridientPos()
        {
            return sauceInSpoonObject.transform.position;
        }
        public override GameObject GetObjectForDrag()
        {
            if(isIngredientDragging) return null;
            isIngredientDragging = true;
            sauceInSpoonObject.SetActive(true);
            spoonTransform.localEulerAngles = dragSpoonRot;
            sauceTransform.localScale = new Vector3(sauceTransform.localScale.x, sauceTransform.localScale.y, sauceTransform.localScale.z - oneItemScaleChange);
            return spoonTransform.gameObject;
        }
        public override void CancelDragObject()
        {
            EndDrag();
            sauceTransform.localScale = new Vector3(sauceTransform.localScale.x, sauceTransform.localScale.y, sauceTransform.localScale.z + oneItemScaleChange);
        }
        public override void RemoveDraggedObject()
        {
            EndDrag();
        }
        void EndDrag()
        {
            if (!isIngredientDragging) return;
            isIngredientDragging = false;
            sauceInSpoonObject.SetActive(false);
            spoonTransform.localEulerAngles = normalSpoonRot;
            spoonTransform.localPosition = normalSpoonPos;
        }
    }
}
