using System.Collections.Generic;
using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaHold.Visual.PizzaContainer
{
    public class PizzaObjsContainer: MonoBehaviour
    {
        public int PizzaCount => pizzasInContainer.Count;
        readonly List<GameObject> pizzasInContainer = new();
        public void AddPizza(GameObject pizzaObj)
        {
            pizzaObj.transform.parent = transform;
            pizzasInContainer.Add(pizzaObj);
        }
        public GameObject GetPizza()
        {
            int pizzaId = PizzaCount - 1;
            GameObject resultPizza = pizzasInContainer[pizzaId];
            pizzasInContainer.RemoveAt(pizzaId);
            return resultPizza;
        }
        public bool TryGetPizza(out GameObject pizzaObj)
        {
            pizzaObj = null;
            if(PizzaCount > 0)
            {
                int pizzaId = PizzaCount - 1;
                pizzaObj = pizzasInContainer[pizzaId];
                pizzasInContainer.RemoveAt(pizzaId);
                return true;
            }
            return false;
        }
    }
}
