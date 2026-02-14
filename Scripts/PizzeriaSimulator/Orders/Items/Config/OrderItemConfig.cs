using System;
using UnityEngine;
namespace Game.PizzeriaSimulator.Orders.Items.Config
{
    [Serializable]
    public sealed class OrderItemConfig
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public PizzeriaOrderItemType OrderItemType { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}
