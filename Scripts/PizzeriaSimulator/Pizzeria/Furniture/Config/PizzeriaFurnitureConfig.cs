using System;
using UnityEngine;
namespace Game.PizzeriaSimulator.Pizzeria.Furniture.Config
{
    [Serializable]
    public sealed class PizzeriaFurnitureConfig
    {
        [SerializeField] PizzeriaFurnitureItemConfig[] furnitureItems;
        public int FurnitureItemsCount => furnitureItems.Length;
        public PizzeriaFurnitureItemConfig GetFurnitureItem(int id)
        {
            if (id < 0 || id >= furnitureItems.Length) return null;
            return furnitureItems[id];
        }
    }
}
