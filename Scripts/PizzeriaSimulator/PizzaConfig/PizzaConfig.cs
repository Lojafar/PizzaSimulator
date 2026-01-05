using Game.PizzeriaSimulator.PaymentReceive;
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
        [field: SerializeField] public PaymentPrice Price { get; private set; }
        [field: SerializeField] public List<PizzaIngredientType>  Ingredients { get; private set; }
        [field: SerializeField] public BakedPizzaObject BakedPizzaPrefab { get; private set; }
        [field: SerializeField] public Sprite PizzaIcon { get; private set; }
        public int GetIndexOfIngredient(PizzaIngredientType ingredientType)
        {
            for(int i = 0; i < Ingredients.Count; i++) if (Ingredients[i] == ingredientType) return i;
            return -1;
        }
    }
}
