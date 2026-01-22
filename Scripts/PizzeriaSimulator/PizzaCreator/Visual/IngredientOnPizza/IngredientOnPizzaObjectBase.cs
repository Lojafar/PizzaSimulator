using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual.IngredientOnPizza
{
    public abstract class IngredientOnPizzaObjectBase : MonoBehaviour
    {
        public abstract void Place(Vector3 placePosition, bool withAnim = true);
    }
}
