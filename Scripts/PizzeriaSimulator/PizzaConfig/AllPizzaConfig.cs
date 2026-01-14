using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.PizzasConfig
{
    [Serializable]
    public class AllPizzaConfig
    {
        [field: SerializeField] public PizzaConfig[] Pizzas { get; private set; }
        public PizzaConfig GetPizzaByID(int id)
        {
            if(id < 0 || id >= Pizzas.Length) return null;
            return Pizzas[id];
        }
    }
}
