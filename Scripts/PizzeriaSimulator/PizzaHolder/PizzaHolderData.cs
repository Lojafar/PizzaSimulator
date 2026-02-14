using System;
using System.Collections.Generic;

namespace Game.PizzeriaSimulator.PizzaHold
{
    [Serializable]
    public sealed class PizzaHolderData
    {
        public readonly Dictionary<int, int> Pizzas = new();

        public readonly Dictionary<int, int> ReservedPizzas = new();
        public PizzaHolderData() { }
        public PizzaHolderData(Dictionary<int, int> _pizzas, Dictionary<int, int> _reservedPizzas)
        {
            Pizzas = _pizzas;
            ReservedPizzas = _reservedPizzas;
        }
        public PizzaHolderData Clone()
        {
            return new PizzaHolderData(new Dictionary<int, int>(Pizzas), new Dictionary<int, int>(ReservedPizzas));
        }
    }
}
