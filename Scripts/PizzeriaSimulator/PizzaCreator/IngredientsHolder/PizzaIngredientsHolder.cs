using Game.PizzeriaSimulator.PizzaCreation.Config;
using Game.Root.ServicesInterfaces;
using System;
using System.Collections.Generic;

namespace Game.PizzeriaSimulator.PizzaCreation.IngredientsHold
{
    public class PizzaIngredientsHolder : IInittable
    {
        public int InitPriority => 11;
        public event Action<PizzaIngredientType, bool> OnIngredientAdded;
        public event Action<PizzaIngredientType> OnIngredientRemoved;
        readonly PizzaCreatorConfig pizzaCreatorConfig;
        readonly IngredientsHolderData ingredientsHolderData;
        public IngredientsHolderData IngredientsHolderData => ingredientsHolderData.Clone();
        public PizzaIngredientsHolder(PizzaCreatorConfig _pizzaCreatorConfig, IngredientsHolderData _ingredientsHolderData)
        {
            pizzaCreatorConfig = _pizzaCreatorConfig;
            ingredientsHolderData = _ingredientsHolderData ?? new IngredientsHolderData() { IngredientsDict = new Dictionary<PizzaIngredientType, int>() { 
                { PizzaIngredientType.Dough, 8},{ PizzaIngredientType.Ketchup, 8}, { PizzaIngredientType.Cheese, 9},{ PizzaIngredientType.Tomato, 9},
                { PizzaIngredientType.Salami, 9}, { PizzaIngredientType.Pepper, 9}, { PizzaIngredientType.Shrimp, 9}} };
        }
        public void Init()
        {
            int i;
            foreach(KeyValuePair<PizzaIngredientType, int> ingredient in ingredientsHolderData.IngredientsDict)
            {
                for (i = 0; i < ingredient.Value; i++)
                {
                    OnIngredientAdded?.Invoke(ingredient.Key, true);
                }
            }
        }
        public bool TryRemoveIngredient(PizzaIngredientType type)
        {
            if(HasIngredient(type))
            {
                OnIngredientRemoved?.Invoke(type); 
                ingredientsHolderData.IngredientsDict[type]--;
                return true;
            }
            return false;
        }
        public bool HasIngredient(PizzaIngredientType type)
        {
            return ingredientsHolderData.IngredientsDict.TryGetValue(type, out int amount) && amount > 0 ;
        }
        public bool TryAddIngredient(PizzaIngredientType type, bool selfAdd = true)
        {
            if (ingredientsHolderData.IngredientsDict.TryGetValue(type, out int amount)) 
            {
                if(amount + 1 > pizzaCreatorConfig.GetIngredientConfigByType(type).MaxAmountInHolder)
                {
                    return false;
                }
                ingredientsHolderData.IngredientsDict[type]++;
            }
            else
            {
                ingredientsHolderData.IngredientsDict.Add(type, 1);
            }
            OnIngredientAdded?.Invoke(type, selfAdd);
            return true;
        }
    }
}
