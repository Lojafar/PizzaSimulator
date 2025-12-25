using System.Collections.Generic;
using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual
{
    class StackedPizzaIngredientContainer : PizzaIngredientContainerBase
    {
        [SerializeField] List<GameObject> ingredients;
        Vector3 dragObjNormalPos;
        public override Vector3 GetDragIngridientPos()
        {
            return ingredients[^1].transform.position;
        }
        public override GameObject GetObjectForDrag()
        {
            dragObjNormalPos = ingredients[^1].transform.position;
            return ingredients[^1];
        }
        public override void CancelDragObject()
        {
            ingredients[^1].transform.position = dragObjNormalPos;
        }
        public override void RemoveDraggedObject()
        {
            Destroy(ingredients[^1]);
            ingredients.RemoveAt(ingredients.Count - 1);
        }
    }
}
