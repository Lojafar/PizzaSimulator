using Game.PizzeriaSimulator.Pizzeria.Furniture.Placement;
using System;
using UnityEngine;
namespace Game.PizzeriaSimulator.Pizzeria.Managment.Expansion
{
    [Serializable]
    public sealed class ExpansionFurnPlaceOverride
    {
        [field: SerializeField] public int OveridePlaceAreaId;
        [field: SerializeField] public FurniturePlaceArea OverriderPlaceArea;
    }
}
