using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaCreation.Config
{
    [Serializable]
    public class PizzaCreatorConfig
    {
        [SerializeField] IngredientConfig[] ingredients;
        [SerializeField] PizzaIngredientType[] ingredientsForBase;
        public IReadOnlyList<IngredientConfig> Ingredients => ingredients;
        public IReadOnlyList<PizzaIngredientType> IngredientsForBase => ingredientsForBase;
        Dictionary<PizzaIngredientType, int> indexByIngredientType;
        public void Init()
        {
            indexByIngredientType = new Dictionary<PizzaIngredientType, int>();
            for(int i = 0; i < ingredients.Length; i++)
            {
                if (Ingredients[i] == null)
                {
                    UnityEngine.Debug.LogWarning($"Null ref in element {i} int PizzaCreatorConfig Ingredients array");
                    continue;
                }
                indexByIngredientType[Ingredients[i].IngredientType] = i;
            }
        }
        public bool IsIngredientOfBaseAndGetIndex(PizzaIngredientType ingredientType, out int index)
        {
            for (int i = 0; i < ingredientsForBase.Length; i++)
            {
                if (ingredientsForBase[i] == ingredientType)
                {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }
        public IngredientConfig GetIngredientConfigByType(PizzaIngredientType ingredientType)
        {
            if (indexByIngredientType == null) Init();
            if(indexByIngredientType.TryGetValue(ingredientType, out int index))
            { 
                return Ingredients[index];
            }
            return null;
        }
    }
}
