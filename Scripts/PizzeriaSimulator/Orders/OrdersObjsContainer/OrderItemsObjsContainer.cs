using Game.PizzeriaSimulator.Orders.Handle;
using Game.PizzeriaSimulator.Orders.Items;
using Game.PizzeriaSimulator.PizzaCreation.Visual;
using Game.PizzeriaSimulator.PizzaHold.Visual;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PizzeriaSimulator.Orders.ObjsContainer
{
    public sealed class OrderItemsObjsContainer : MonoBehaviour
    {
        readonly Dictionary<PizzeriaOrderItemType, List<GameObject>> itemsByType = new(possibleDifferentItems);
        const int possibleDifferentItems = 6;
        const int possibleSameItems = 3;
        public bool TryGetAndRemoveItem(PizzeriaOrderItemType itemType, out GameObject itemObject)
        {
            if(itemsByType.TryGetValue(itemType, out List<GameObject> items) && items.Count > 0)
            {
                itemObject = items[^1];
                items.RemoveAt(items.Count - 1);
                return true;
            }
            itemObject = null;
            return false;
        }
        public void AddPizza(PizzaBox pizzaObject)
        {
            PizzeriaOrderItemType itemType = PizzeriaOrdersHandler.PizzaIdToOrderItemType(pizzaObject.PizzaID);
            AddItem(itemType, pizzaObject.gameObject);
        }
        public void AddSodaCup(GameObject cupGameobject)
        {
            AddItem(PizzeriaOrderItemType.Soda, cupGameobject);
        }
        public void AddItem(PizzeriaOrderItemType itemType, GameObject itemObject)
        {
            itemObject.transform.parent = transform;
            if (!itemsByType.ContainsKey(itemType)) itemsByType.Add(itemType, new List<GameObject>(possibleSameItems));
            itemsByType[itemType].Add(itemObject);
        }
        public void DestroyAllItems()
        {
            int i;
            foreach (List<GameObject> items in itemsByType.Values)
            {
                for (i = 0; i < items.Count; i++)
                {
                    Destroy(items[i]);
                }
                items.Clear();
            }
            itemsByType.Clear();
        }
    }
}
