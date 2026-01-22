using Game.PizzeriaSimulator.Boxes;
using Game.PizzeriaSimulator.Currency;
using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Delivery.Config
{
    [Serializable]
    public class DeliveryItemConfig
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public int QuantityByOrder { get; private set; }
        [field: SerializeField] public MoneyQuantity Price { get; private set; }
        [field: SerializeField] public Sprite ItemIcon { get; private set; }
        [field: SerializeField] public CarriableBoxBase BoxPrefab { get; private set; }
    }
}
