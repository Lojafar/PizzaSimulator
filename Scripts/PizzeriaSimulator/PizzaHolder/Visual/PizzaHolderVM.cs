using Game.PizzeriaSimulator.PizzasConfig;
using Game.Root.ServicesInterfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaHold.Visual
{
    public class PizzaHolderVM : ISceneDisposable
    {
        public event Action<Sprite> AddPizza;
        public event Action<int> RemovePizza;
        readonly PizzaHolder pizzaHolder;
        readonly AllPizzaConfig allPizzaConfig;
        readonly List<int> pizzas;
        public PizzaHolderVM(PizzaHolder _pizzaHolder, AllPizzaConfig _allPizzaConfig)
        {
            pizzaHolder = _pizzaHolder;
            allPizzaConfig = _allPizzaConfig;
            pizzas = new List<int>();
        }
        public void Init()
        {
            pizzaHolder.OnPizzaAdded += HandleNewPizza;
            pizzaHolder.OnPizzaRemoved += HandlePizzaRemove;
        }
        public void Dispose()
        {
            pizzaHolder.OnPizzaAdded -= HandleNewPizza;
            pizzaHolder.OnPizzaRemoved -= HandlePizzaRemove;
        }
        void HandleNewPizza(int pizzaID)
        {
            pizzas.Add(pizzaID);
            if (allPizzaConfig.GetPizzaByID(pizzaID) is PizzaConfig pizzaConfig)
            {
                AddPizza?.Invoke(pizzaConfig.PizzaIcon);
            }
        }
        void HandlePizzaRemove(int pizzaID)
        {
            for (int i = 0; i < pizzas.Count; i++) 
            {
                if(pizzas[i] == pizzaID)
                {
                    RemovePizza?.Invoke(i);
                    pizzas.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
