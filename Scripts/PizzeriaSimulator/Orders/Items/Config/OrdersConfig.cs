using System;
using System.Collections.Generic;
using UnityEngine;
namespace Game.PizzeriaSimulator.Orders.Items.Config
{
    [Serializable]
    public sealed class OrdersConfig
    {
        [SerializeField] OrderItemConfig[] itemsConfigs;
        [field: SerializeField] public Sprite PizzaBakeIndicator { get; private set; }
        [field: SerializeField] public Sprite PizzaCutIndicator  {get; private set;}
        [field: SerializeField] public Sprite OrderReadyIndicator{get; private set;}
        readonly Dictionary<PizzeriaOrderItemType, int> itemIndexByType = new ();
        public void Init()
        {
            for (int i = 0; i < itemsConfigs.Length; i++) 
            {
                itemIndexByType[itemsConfigs[i].OrderItemType] = i;
            }
        }
        public OrderItemConfig GetOrderItemById(int id) 
        {
            if (id < 0 || id >= itemsConfigs.Length) return null;
            return itemsConfigs[id];
        }
        public OrderItemConfig GetOrderItemByType(PizzeriaOrderItemType itemType)
        {
            if(itemIndexByType.TryGetValue(itemType, out int index)) return itemsConfigs[index];
            return null;
        }
    }
}
