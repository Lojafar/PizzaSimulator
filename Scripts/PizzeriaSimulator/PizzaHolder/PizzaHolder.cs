using Game.Root.ServicesInterfaces;
using System;
using System.Collections.Generic;

namespace Game.PizzeriaSimulator.PizzaHold
{
    public class PizzaHolder : IInittable
    {
        public event Action<int> OnPizzaAdded;
        public event Action<int> OnPizzaReserved;
        public event Action<int> OnPizzaRemoved;
        readonly PizzaHolderData pizzaHolderData;
        public PizzaHolderData PizzaHolderData => pizzaHolderData.Clone();
        public PizzaHolder(PizzaHolderData _pizzaHolderData)
        {
            pizzaHolderData = _pizzaHolderData ?? new PizzaHolderData();
        }
        public void Init()
        {
            foreach (KeyValuePair<int, int> pizza in pizzaHolderData.ReservedPizzas)
            {
                if (pizzaHolderData.Pizzas.ContainsKey(pizza.Key))
                {
                    pizzaHolderData.Pizzas[pizza.Key] += pizza.Value;
                }
                else
                {
                    pizzaHolderData.Pizzas.Add(pizza.Key, pizza.Value);
                }
            }
            pizzaHolderData.ReservedPizzas.Clear();
            int pizzaInd;
            foreach (KeyValuePair<int, int> pizza in pizzaHolderData.Pizzas)
            {
                for (pizzaInd = 0; pizzaInd < pizza.Value; pizzaInd++)
                {
                    OnPizzaAdded?.Invoke(pizza.Key);
                }
            }
           
        }
        public void AddPizza(int pizzaID)
        {
            if (pizzaHolderData.Pizzas.ContainsKey(pizzaID)) 
            {
                pizzaHolderData.Pizzas[pizzaID]++;
            }
            else
            {
                pizzaHolderData.Pizzas.Add(pizzaID, 1);
            }
            OnPizzaAdded?.Invoke(pizzaID);
        }
        public bool TryRemovePizza(int pizzaID)
        {
            if (HasPizza(pizzaID))
            {
                pizzaHolderData.Pizzas[pizzaID]--;
                OnPizzaRemoved?.Invoke(pizzaID);
                return true;
            }
            return false;
        }
        public bool TryReservePizza(int pizzaID)
        {
            if (HasPizza(pizzaID))
            {
                pizzaHolderData.Pizzas[pizzaID]--;
                if (!pizzaHolderData.ReservedPizzas.ContainsKey(pizzaID))
                {
                    pizzaHolderData.ReservedPizzas[pizzaID] = 1;
                }
                else
                {
                    pizzaHolderData.ReservedPizzas[pizzaID]++;
                }
                OnPizzaReserved?.Invoke(pizzaID);
                return true;
            }
            return false;
        }
        public void RemoveReservedPizza(int pizzaID)
        {
            if (pizzaHolderData.ReservedPizzas.TryGetValue(pizzaID, out int amount) && amount > 0)
            {
                pizzaHolderData.ReservedPizzas[pizzaID]--;
                OnPizzaRemoved?.Invoke(pizzaID);
            }
        }
        public bool HasPizza(int pizzaID)
        {
            return pizzaHolderData.Pizzas.TryGetValue(pizzaID, out int amount) && amount > 0;
        }
    }
}
