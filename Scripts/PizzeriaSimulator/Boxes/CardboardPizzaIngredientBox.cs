using Game.PizzeriaSimulator.Boxes.Item.PizzaIngredient;
using Game.PizzeriaSimulator.PizzaCreation;
using System.Collections.Generic;
using UnityEngine;
namespace Game.PizzeriaSimulator.Boxes
{
    sealed class CardboardPizzaIngredientBox : CardboardCarriableBoxBase, IPizzaIngredientsBox
    {
        [field: SerializeField]  public PizzaIngredientType IngredientType { get; private set; }
        [SerializeField] List<PizzaIngredientBoxItemBase> items;
        public override CarriableBoxType BoxType => CarriableBoxType.PizzaIngredientsBox;
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
        public PizzaIngredientBoxItemBase RemoveAndGetItem()
        {
            if (items.Count < 1) return null;
            PizzaIngredientBoxItemBase boxItem = items[^1];
            items.RemoveAt(items.Count - 1);
            boxData.ItemsAmount = items.Count;
            return boxItem;
        }
    }
}
