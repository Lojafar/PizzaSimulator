using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaCreation.Config
{
    [Serializable]
    public class PizzaCreatorConfig
    {
        [field: SerializeField] public IngredientConfig[] Ingredients { get; private set; }
        [field: SerializeField] public PizzaConfig[] Pizzas { get; private set; }
        [field: SerializeField] public PizzaIngredientType[] IngredientsForBase { get; private set; }
        Dictionary<PizzaIngredientType, int> indexByIngredientType;
        public void Init()
        {
            indexByIngredientType = new Dictionary<PizzaIngredientType, int>();
            for(int i = 0; i < Ingredients.Length; i++)
            {
                if (Ingredients[i] == null)
                {
                    UnityEngine.Debug.LogWarning($"Null ref in element {i} int PizzaCreatorConfig Ingredients array");
                    continue;
                }
                indexByIngredientType[Ingredients[i].IngredientType] = i;
            }
        }
        public int GetBaseIngredientIndex(PizzaIngredientType ingredientType)
        {
            for(int i = 0;i < IngredientsForBase.Length; i++) if(IngredientsForBase[i] == ingredientType) return i;
            return -1;
        }
        public bool IsIngredientOfBaseAndGetIndex(PizzaIngredientType ingredientType, out int index)
        {
            for (int i = 0; i < IngredientsForBase.Length; i++)
            {
                if (IngredientsForBase[i] == ingredientType)
                {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }
        public IngredientConfig GetIngredientConfigByType(PizzaIngredientType ingredientTypes)
        {
            if (indexByIngredientType == null) Init();
            if(indexByIngredientType.TryGetValue(ingredientTypes, out int index))
            { 
                return Ingredients[index];
            }
            return null;
        }
        public PizzaConfig GetPizzaByID(int id)
        {
            return Pizzas[id];
        }
    }
}
