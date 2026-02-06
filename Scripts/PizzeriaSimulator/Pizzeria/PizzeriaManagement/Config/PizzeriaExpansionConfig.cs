using Game.PizzeriaSimulator.Currency;
using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Pizzeria.Managment.Config
{
    [Serializable]
    public sealed class PizzeriaExpansionConfig
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public MoneyQuantity Price { get; private set; }
    }
}
