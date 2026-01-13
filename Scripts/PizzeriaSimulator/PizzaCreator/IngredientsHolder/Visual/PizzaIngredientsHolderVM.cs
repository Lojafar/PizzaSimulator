using Game.Root.ServicesInterfaces;
using System;

namespace Game.PizzeriaSimulator.PizzaCreation.IngredientsHold.Visual
{
    public class PizzaIngredientsHolderVM : ISceneDisposable
    {
        public event Action<PizzaIngredientType> AddIngredient;
        public event Action<PizzaIngredientType> SpawnIngredient;
        public event Action<PizzaIngredientType> RemoveIngredient;
        readonly PizzaIngredientsHolder pizzaIngredientsHolder;
        public PizzaIngredientsHolderVM(PizzaIngredientsHolder _pizzaIngredientsHolder)
        {
            pizzaIngredientsHolder = _pizzaIngredientsHolder;
        }
        public void Init()
        {
            pizzaIngredientsHolder.OnIngredientAdded += HandleIngredientAdd;
            pizzaIngredientsHolder.OnIngredientRemoved += HandleIngredientRemove;
        }
        public void Dispose() 
        {
            pizzaIngredientsHolder.OnIngredientAdded -= HandleIngredientAdd;
            pizzaIngredientsHolder.OnIngredientRemoved -= HandleIngredientRemove;
        }
        void HandleIngredientAdd(PizzaIngredientType ingredientType, bool selfAdd) 
        {
            AddIngredient?.Invoke(ingredientType);
            if (selfAdd) SpawnIngredient?.Invoke(ingredientType);
        }
        void HandleIngredientRemove(PizzaIngredientType ingredientType)
        {
            RemoveIngredient?.Invoke(ingredientType);
        }
    }
}
