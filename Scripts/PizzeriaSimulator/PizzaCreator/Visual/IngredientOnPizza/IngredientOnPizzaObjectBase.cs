using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual.IngredientOnPizza
{
    public abstract class IngredientOnPizzaObjectBase : MonoBehaviour
    {
        public abstract void DoPlaceAnim(Vector3 placePosition);
    }
}
