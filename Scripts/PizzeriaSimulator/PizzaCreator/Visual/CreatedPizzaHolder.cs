using Game.PizzeriaSimulator.PizzaCreation.Visual.IngredientOnPizza;
using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual
{
    class CreatedPizzaHolder : MonoBehaviour
    {
        [SerializeField] Transform pizzaPositionTransform;
        [SerializeField] PizzaObject pizzaObjPrefab;
        public PizzaObject SpawnNewPizzaObject()
        {
            return Instantiate(pizzaObjPrefab, pizzaPositionTransform.position, pizzaObjPrefab.transform.rotation);
        }
        public void SpawnAndAddIngredient(IngredientOnPizzaObjectBase ingredientPrefab, Vector3 startPosition, PizzaObject pizzaParent)
        {
            IngredientOnPizzaObjectBase spawnedIngredient = Instantiate(ingredientPrefab, startPosition, ingredientPrefab.transform.rotation);
            spawnedIngredient.transform.parent = pizzaParent.transform;
            spawnedIngredient.DoPlaceAnim(pizzaPositionTransform.position);
        }
    }
}
