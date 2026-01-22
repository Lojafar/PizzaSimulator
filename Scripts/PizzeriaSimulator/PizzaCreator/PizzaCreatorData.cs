using System;
using System.Collections.Generic;

namespace Game.PizzeriaSimulator.PizzaCreation
{
    [Serializable]
    public class PizzaCreatorData
    {
        public readonly List<int> PizzasInBake = new (4);
        public int PizzaInCut = -1;
        public PizzaCreatorData() { }
        public PizzaCreatorData(List<int> _pizzasInBake, int _pizzaInCut) 
        {
            PizzasInBake = _pizzasInBake;
            PizzaInCut= _pizzaInCut;
        }
        public PizzaCreatorData Clone()
        {
            return new PizzaCreatorData(new List<int>(PizzasInBake), PizzaInCut);
        }
    }
}
