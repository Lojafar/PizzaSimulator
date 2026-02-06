using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Delivery.Config
{
    [Serializable]
    public class PizzeriaDeliveryConfig
    {
        [SerializeField] DeliveryItemConfig[] itemsConfigs;
        [field: SerializeField] public AnimationCurve DeliveryTimeСoeffCurve { get; private set; }
        [field: SerializeField] public int MinDeliveryTime { get; private set; }
        [field: SerializeField] public int MaxDeliveryTime { get; private set; }
        [field: SerializeField] public int SkipStepDuration { get; private set; }
        [field: SerializeField] public int SkipStepGemsChange { get; private set; }
        public int ItemsAmount => itemsConfigs.Length;
        public DeliveryItemConfig GetDeliveryItemConfig(int id)
        {
            if(id < 0 || id >= itemsConfigs.Length) return null;
            return itemsConfigs[id];
        }
    }
}
