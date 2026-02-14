using Game.PizzeriaSimulator.Boxes.Item.Furniture;
namespace Game.PizzeriaSimulator.Boxes
{
    public interface IFurnitureBox
    {
        public int FurnitureID { get; }
        public FurnitureBoxItemBase GetFurnitureItem();
        public void RemoveItem();
    }
}
