using UnityEngine;
namespace Game.PizzeriaSimulator.PizzaCreation.IngredientsHold.Visual
{
    class PizzaIngredientsHolderView : PizzaIngredientsHolderViewBase
    {
        [SerializeField] PizzaIngredientContainerBase[] containersInOrder; 
        public override Vector3 GetNextIngredientPos(PizzaIngredientType ingredientType)
        {
            int containerIndex = (int)ingredientType;
            if (containerIndex < containersInOrder.Length)
            {
               return containersInOrder[containerIndex].GetNextIngredientPos();
            }
            return Vector3.zero;
        }
        public override void SetIngredientToContainer(PizzaIngredientType ingredientType, GameObject gameObject)
        {
            int containerIndex = (int)ingredientType;
            if (containerIndex < containersInOrder.Length)
            {
                containersInOrder[containerIndex].AddIngredient(gameObject);
            }
        }
        public override void Bind(PizzaIngredientsHolderVM _viewModel)
        {
            base.Bind(_viewModel);
            viewModel.SpawnIngredient += AddIngredientOfType;
            viewModel.RemoveIngredient += RemoveIngredientOfType;
        }
        void AddIngredientOfType(PizzaIngredientType ingredientType)
        {
            int containerIndex = (int)ingredientType;
            if (containerIndex < containersInOrder.Length) 
            {
                containersInOrder[containerIndex].AddIngredient();
            }
        }
        void RemoveIngredientOfType(PizzaIngredientType ingredientType)
        {
            int containerIndex = (int)ingredientType;
            if (containerIndex < containersInOrder.Length)
            {
                containersInOrder[containerIndex].RemoveIngredient();
            }
        }

        
    }
}
