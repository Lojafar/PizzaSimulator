using Game.PizzeriaSimulator.Currency;
using Game.PizzeriaSimulator.PizzaCreation;
using Game.PizzeriaSimulator.PizzaCreation.Visual;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Game.PizzeriaSimulator.PizzasConfig
{
    [Serializable]
    public class PizzaConfig
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public MoneyQuantity Price { get; private set; }
        [SerializeField] List<PizzaIngredientType> ingredients;
        [field: SerializeField] public BakedPizzaObject BakedPizzaPrefab { get; private set; }
        [field: SerializeField] public Sprite PizzaIcon { get; private set; }
        public IReadOnlyList<PizzaIngredientType> Ingredients => ingredients;
        public bool ContainsIngredient(PizzaIngredientType ingredientType)
        {
            return ingredients.Contains(ingredientType);
        }
        public int GetIndexOfIngredient(PizzaIngredientType ingredientType)
        {
            for(int i = 0; i < ingredients.Count; i++) if (ingredients[i] == ingredientType) return i;
            return -1;
        }
    }
}
