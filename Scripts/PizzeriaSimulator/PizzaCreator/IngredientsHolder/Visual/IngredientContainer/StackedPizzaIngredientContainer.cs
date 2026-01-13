using System.Collections.Generic;
using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaCreation.IngredientsHold.Visual
{
    class StackedPizzaIngredientContainer : PizzaIngredientContainerBase
    {
        [SerializeField] GameObject ingredientPrefab;
        [SerializeField] float xSpacing;
        [SerializeField] float zSpacing;
        [SerializeField] int maxAmountOnZ;
        [SerializeField] Vector3 startLocalPos;
        [SerializeField] Transform ingredientsContainer;
        readonly List<GameObject> ingredients = new();
        Vector3 dragObjNormalPos; 
        public override void AddIngredient()
        {
            GameObject spawnedIngredient = Instantiate(ingredientPrefab, ingredientsContainer);
            spawnedIngredient.transform.localPosition = GetNextIngredientLocalPos();
            ingredients.Add(spawnedIngredient);
        }
        public override void AddIngredient(GameObject gameObject)
        {
            gameObject.transform.parent = ingredientsContainer;
            gameObject.transform.localRotation = ingredientPrefab.transform.localRotation;
            ingredients.Add(gameObject);
        }
        public override void RemoveIngredient()
        {
            if (ingredients.Count < 1) return;
            Destroy(ingredients[^1]);
            ingredients.RemoveAt(ingredients.Count - 1);
        }
       
        public override Vector3 GetDragIngredientPos()
        {
            if (ingredients.Count < 1) return Vector3.zero;
            return ingredients[^1].transform.position;
        }
        public Vector3 GetNextIngredientLocalPos()
        {
            int xNumber = Mathf.FloorToInt((float)ingredients.Count / maxAmountOnZ);
            int zNumber = ingredients.Count - xNumber * maxAmountOnZ;
            return new Vector3(startLocalPos.x + xNumber * xSpacing, startLocalPos.y, startLocalPos.z + zNumber * zSpacing);
        }
        public override Vector3 GetNextIngredientPos()
        {
            return GetNextIngredientLocalPos() + ingredientsContainer.position;
        }
        public override GameObject GetObjectForDrag()
        {
            if (ingredients.Count < 1) return null;
            dragObjNormalPos = ingredients[^1].transform.position;
            return ingredients[^1];
        }
        public override void EndDragObject()
        {
            if (ingredients.Count < 1) return;
            ingredients[^1].transform.position = dragObjNormalPos;
        }
    }
}
