using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaCreation.IngredientsHold.Visual
{
    class SaucePizzaIngredientContainer : PizzaIngredientContainerBase
    {
        [SerializeField] Transform sauceTransform;
        [SerializeField] Transform spoonTransform;
        [SerializeField] Transform minBound;
        [SerializeField] Transform maxBound;
        [SerializeField] GameObject sauceInSpoonObject;
        [SerializeField] Vector3 oneItemScaleChange;
        [SerializeField] Vector3 normalSpoonRot;
        [SerializeField] Vector3 normalSpoonPos;
        [SerializeField] Vector3 dragSpoonRot;
        bool isIngredientDragging;
        public override void AddIngredient() 
        {
            sauceTransform.localScale = new Vector3(sauceTransform.localScale.x, sauceTransform.localScale.y, sauceTransform.localScale.z) + oneItemScaleChange;
        }
        public override void AddIngredient(GameObject gameObject)
        {
            AddIngredient();
        }
        public override void RemoveIngredient()
        {
            sauceTransform.localScale = new Vector3(sauceTransform.localScale.x, sauceTransform.localScale.y, sauceTransform.localScale.z) - oneItemScaleChange;
        }
        public override Vector3 GetDragIngredientPos()
        {
            return sauceInSpoonObject.transform.position;
        }
        public override Vector3 GetNextIngredientPos()
        {
            return new Vector3(Random.Range(minBound.position.x, maxBound.position.x), sauceTransform.position.y, Random.Range(minBound.position.z, maxBound.position.z));
        }
        public override GameObject GetObjectForDrag()
        {
            if(isIngredientDragging) return null;
            isIngredientDragging = true;
            sauceInSpoonObject.SetActive(true);
            spoonTransform.localEulerAngles = dragSpoonRot;
            sauceTransform.localScale = new Vector3(sauceTransform.localScale.x, sauceTransform.localScale.y, sauceTransform.localScale.z) - oneItemScaleChange;
            return spoonTransform.gameObject;
        }
        public override void EndDragObject()
        {
            if (!isIngredientDragging) return;
            isIngredientDragging = false;
            sauceInSpoonObject.SetActive(false);
            spoonTransform.localEulerAngles = normalSpoonRot;
            spoonTransform.localPosition = normalSpoonPos;
            sauceTransform.localScale = new Vector3(sauceTransform.localScale.x, sauceTransform.localScale.y, sauceTransform.localScale.z) + oneItemScaleChange;
        }
    }
}
