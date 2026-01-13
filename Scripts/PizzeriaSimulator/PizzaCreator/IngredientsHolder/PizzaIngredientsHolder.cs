using Game.PizzeriaSimulator.PizzaCreation.Config;
using Game.Root.ServicesInterfaces;
using System;
using System.Collections.Generic;

namespace Game.PizzeriaSimulator.PizzaCreation.IngredientsHold
{
    public class PizzaIngredientsHolder : IInittable
    {
        public event Action<PizzaIngredientType, bool> OnIngredientAdded;
        public event Action<PizzaIngredientType> OnIngredientRemoved;
        readonly PizzaCreatorConfig pizzaCreatorConfig;
        readonly Dictionary<PizzaIngredientType, int> ingredientsDict;

        public PizzaIngredientsHolder(PizzaCreatorConfig _pizzaCreatorConfig) 
        {
            pizzaCreatorConfig = _pizzaCreatorConfig;
            ingredientsDict = new Dictionary<PizzaIngredientType, int>();
            ingredientsDict.Add(PizzaIngredientType.Salami, 12);
            ingredientsDict.Add(PizzaIngredientType.Tomato, 12);
            ingredientsDict.Add(PizzaIngredientType.Shrimp, 12);
            ingredientsDict.Add(PizzaIngredientType.Pepper, 12);
        }
        public void Init()
        { 
            for(int i = 0; i < 12; i++)
            {
                 OnIngredientAdded?.Invoke(PizzaIngredientType.Salami, true);
                 OnIngredientAdded?.Invoke(PizzaIngredientType.Tomato, true);
                 OnIngredientAdded?.Invoke(PizzaIngredientType.Shrimp, true);
                 OnIngredientAdded?.Invoke(PizzaIngredientType.Pepper, true);
            }
        }
        public bool TryRemoveIngredient(PizzaIngredientType type)
        {
            if(HasIngredient(type))
            {
                OnIngredientRemoved?.Invoke(type);
                ingredientsDict[type]--;
                return true;
            }
            return false;
        }
        public bool HasIngredient(PizzaIngredientType type)
        {
            return ingredientsDict.TryGetValue(type, out int amount) && amount > 0 ;
        }
        public bool TryAddIngredient(PizzaIngredientType type, bool selfAdd = true)
        {
            if (ingredientsDict.TryGetValue(type, out int amount)) 
            {
                if(amount + 1 > pizzaCreatorConfig.GetIngredientConfigByType(type).MaxAmountInHolder)
                {
                    return false;
                }
                ingredientsDict[type]++;
            }
            else
            {
                ingredientsDict.Add(type, 1);
            }
            OnIngredientAdded?.Invoke(type, selfAdd);
            return true;
        }
    }
}
