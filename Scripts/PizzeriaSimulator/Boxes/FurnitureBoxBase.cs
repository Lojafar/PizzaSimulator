using Game.PizzeriaSimulator.Boxes.Item;
using UnityEngine;
namespace Game.PizzeriaSimulator.Boxes
{
    public abstract class FurnitureBoxBase : CarriableBoxBase
    {
        [field: SerializeField] public int FurnitureID { get; protected set; }
        public abstract FurnitureBoxItemBase GetFurnitureItem();
        public abstract void RemoveItem();
    }
}
