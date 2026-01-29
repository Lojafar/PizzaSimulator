using Game.PizzeriaSimulator.Boxes.Item;
using Game.PizzeriaSimulator.Interactions;
using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold;
using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold.Visual;
using UnityEngine;

namespace Game.PizzeriaSimulator.Boxes.Carry.Handler
{
    class PizzaIngredientBoxHandler : IBoxesHandler
    {
        readonly BoxesCarrier boxesCarrier;
        readonly PizzaIngredientsHolder ingredientsHolder;
        readonly PizzaIngredientsHolderViewBase ingredientsHolderView;
        PizzaIngredientBoxBase activeBox;
        public PizzaIngredientBoxHandler(BoxesCarrier _boxesCarrier ,PizzaIngredientsHolder _ingredientsHolder, PizzaIngredientsHolderViewBase _ingredientsHolderView)
        {
            boxesCarrier = _boxesCarrier;
            ingredientsHolder = _ingredientsHolder;
            ingredientsHolderView = _ingredientsHolderView;
        }

        public void SetBox(CarriableBoxBase box)
        {
            if(box is PizzaIngredientBoxBase pizzaIngredientBox)
            {
                activeBox = pizzaIngredientBox;
                return;
            }
            activeBox = null;
        }
        public void HandleInteraction(InteractableType interactableType, GameObject interactedObject)
        {
            if (interactableType == InteractableType.PizzaCreateTable)
            {
                OnIngredientHolderInteract();
            }
        }
        void OnIngredientHolderInteract()
        {
            if (activeBox.ItemsAmount < 1)
            {
                boxesCarrier.OnDenyAction("Empty box");
                return;
            }
            if (!activeBox.IsOpened)
            {
                boxesCarrier.OnDenyAction("Before unpack the box");
                return;
            }
            if (ingredientsHolder.TryAddIngredient(activeBox.IngredientType, false))
            {
                BoxItemBase item = activeBox.RemoveAndGetItem();
                item.SetTo(ingredientsHolderView.GetNextIngredientPos(activeBox.IngredientType),
                   () => ingredientsHolderView.SetIngredientToContainer(activeBox.IngredientType, item.gameObject));
            }
            else
            {
                boxesCarrier.OnDenyAction("Full ingredient container");
            }
        }
    }
}
