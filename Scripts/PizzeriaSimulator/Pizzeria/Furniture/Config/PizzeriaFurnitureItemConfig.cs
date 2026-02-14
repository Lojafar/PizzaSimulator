using System;
using UnityEngine;
namespace Game.PizzeriaSimulator.Pizzeria.Furniture.Config
{
    [Serializable]
    public sealed class PizzeriaFurnitureItemConfig
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public int PlaceAreaID { get; private set; }
        [field: SerializeField] public GameObject FurniturePrefab { get; private set; }
    }
}
