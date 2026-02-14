using Game.PizzeriaSimulator.Boxes.Item.Consumable;

namespace Game.PizzeriaSimulator.Boxes
{
    public interface IConsumablesBox
    {
        public ConsumableBoxItemType ConsumableItemType { get; }
        public ConsumableBoxItemBase RemoveAndGetItem();
    }
}
