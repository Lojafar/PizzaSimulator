using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Delivery.Config
{
    [Serializable]
    public class PizzeriaDeliveryConfig
    {
        [SerializeField] DeliveryItemConfig[] itemsConfigs;
        public int ItemsAmount => itemsConfigs.Length;
        public DeliveryItemConfig GetDeliveryItemConfig(int id)
        {
            if(id < 0 || id >= itemsConfigs.Length) return null;
            return itemsConfigs[id];
        }
    }
}
