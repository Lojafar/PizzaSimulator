using System;
using System.Collections.Generic;
using Game.Root.ServicesInterfaces;

namespace Game.PizzeriaSimulator.PizzaHold.Visual
{
    public class PizzaHolderVM : ISceneDisposable
    {
        public event Action AddPizza;
        public event Action<int> RemovePizza;
        readonly PizzaHolder pizzaHolder;
        readonly List<int> pizzas;
        public PizzaHolderVM(PizzaHolder _pizzaHolder)
        {
            pizzaHolder = _pizzaHolder;
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
            AddPizza?.Invoke();
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
