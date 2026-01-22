using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.PizzasConfig
{
    [Serializable]
    public class AllPizzaConfig
    {
        [SerializeField] PizzaConfig[] Pizzas;
        public int PizzasCount => Pizzas.Length;
        public PizzaConfig GetPizzaByID(int id)
        {
            if(id < 0 || id >= Pizzas.Length) return null;
            return Pizzas[id];
        }
    }
}
