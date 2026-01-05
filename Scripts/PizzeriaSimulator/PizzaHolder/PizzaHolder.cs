using System;
using System.Collections.Generic;

namespace Game.PizzeriaSimulator.PizzaHold
{
    public class PizzaHolder
    {
        public event Action<int> OnPizzaAdded;
        public event Action<int> OnPizzaRemoved;
        readonly Dictionary<int, int> pizzas;
        readonly Dictionary<int, int> reservedPizzas;
        public PizzaHolder()
        {
            pizzas = new Dictionary<int, int>();
            reservedPizzas = new Dictionary<int, int>();
        }
        public void AddPizza(int pizzaID)
        {
            if (pizzas.ContainsKey(pizzaID)) 
            {
                pizzas[pizzaID]++;
            }
            else
            {
                pizzas.Add(pizzaID, 1);
            }
            OnPizzaAdded?.Invoke(pizzaID);
        }
        public bool TryRemovePizza(int pizzaID)
        {
            if (HasPizza(pizzaID))
            {
                pizzas[pizzaID]--;
                OnPizzaRemoved?.Invoke(pizzaID);
                return true;
            }
            return false;
        }
        public bool TryReservePizza(int pizzaID)
        {
            if (HasPizza(pizzaID))
            {
                pizzas[pizzaID]--;
                if (!reservedPizzas.ContainsKey(pizzaID))
                {
                    reservedPizzas[pizzaID] = 1;
                }
                else
                {
                    reservedPizzas[pizzaID]++;
                }
                return true;
            }
            return false;
        }
        public void RemoveReservedPizza(int pizzaID)
        {
            if (reservedPizzas.TryGetValue(pizzaID, out int amount) && amount > 0)
            {
                reservedPizzas[pizzaID]--;
                OnPizzaRemoved?.Invoke(pizzaID);
            }
        }
        public bool HasPizza(int pizzaID)
        {
            return pizzas.TryGetValue(pizzaID, out int amount) && amount > 0;
        }
    }
}
