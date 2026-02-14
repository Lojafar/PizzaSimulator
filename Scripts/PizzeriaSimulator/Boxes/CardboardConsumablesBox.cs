using Game.PizzeriaSimulator.Boxes.Item.Consumable;
using System.Collections.Generic;
using UnityEngine;
namespace Game.PizzeriaSimulator.Boxes
{
    sealed class CardboardConsumablesBox : CardboardCarriableBoxBase, IConsumablesBox
    {
        [field: SerializeField] public ConsumableBoxItemType ConsumableItemType { get; private set; }
        [SerializeField] List<ConsumableBoxItemBase> items;
        public override CarriableBoxType BoxType => CarriableBoxType.ConsumablesBox;
        protected override void Awake()
        {
            base.Awake();
            boxData.ItemsAmount = items.Count;
        }
        public override void SetBoxData(CarriableBoxData _boxData)
        {
            base.SetBoxData(_boxData);
            for (int itemsToRemove = items.Count - boxData.ItemsAmount; itemsToRemove > 0; itemsToRemove--)
            {
                Destroy(items[^1].gameObject);
                items.RemoveAt(items.Count - 1);
            }
        }
        public ConsumableBoxItemBase RemoveAndGetItem()
        {
            if (items.Count < 1) return null;
            ConsumableBoxItemBase boxItem = items[^1];
            items.RemoveAt(items.Count - 1);
            boxData.ItemsAmount = items.Count;
            return boxItem;
        }
    }
}
