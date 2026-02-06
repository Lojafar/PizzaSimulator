using System;
using System.Collections.Generic;
namespace Game.PizzeriaSimulator.Pizzeria.Furniture.Placement.Manager
{
    [Serializable]
    public sealed class PizzeriaFurnitureManagerData
    {
        public List<PlacedFurnitureData> PlacedFurniture = new();
        public PizzeriaFurnitureManagerData()
        {

        }
        public PizzeriaFurnitureManagerData(List<PlacedFurnitureData> _placedFurniture)
        {
            PlacedFurniture= _placedFurniture;
        }
        public PizzeriaFurnitureManagerData Clone()
        {
            return new PizzeriaFurnitureManagerData(new List<PlacedFurnitureData>(PlacedFurniture));
        }

    }
}
