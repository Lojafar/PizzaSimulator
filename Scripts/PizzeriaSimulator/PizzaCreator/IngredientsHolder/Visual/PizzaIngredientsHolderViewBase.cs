using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaCreation.IngredientsHold.Visual
{
    public abstract class PizzaIngredientsHolderViewBase : MonoBehaviour
    {
        protected PizzaIngredientsHolderVM viewModel;
        public virtual void Bind(PizzaIngredientsHolderVM _viewModel)
        {
            viewModel = _viewModel;
        }
        public abstract Vector3 GetNextIngredientPos(PizzaIngredientType ingredientType);
        public abstract void SetIngredientToContainer(PizzaIngredientType ingredientType, GameObject gameObject);
    }
}
