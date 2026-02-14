using System;
using System.Collections.Generic;

namespace Game.PizzeriaSimulator.PizzaCreation.IngredientsHold
{
    [Serializable]
    public class IngredientsHolderData
    {
        public Dictionary<PizzaIngredientType, int> IngredientsDict = new();
        public IngredientsHolderData() { }
        public IngredientsHolderData(Dictionary<PizzaIngredientType, int> ingredients) 
        {
            IngredientsDict = ingredients;
        }
        public IngredientsHolderData Clone()
        {
            return new IngredientsHolderData(new Dictionary<PizzaIngredientType, int>(IngredientsDict));
        }
    }
}
