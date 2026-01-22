using Game.PizzeriaSimulator.Boxes.Item;
using Game.PizzeriaSimulator.Interactions;
using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold;
using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold.Visual;
using Game.Root.Utils;
using UnityEngine;

namespace Game.PizzeriaSimulator.Boxes.Carry.Handler
{
    class PizzaIngredientBoxHandler : IBoxesHandler
    {
        readonly PizzaIngredientsHolder ingredientsHolder;
        readonly PizzaIngredientsHolderViewBase ingredientsHolderView;
        PizzaIngredientBoxBase activeBox;
        public PizzaIngredientBoxHandler(PizzaIngredientsHolder _ingredientsHolder, PizzaIngredientsHolderViewBase _ingredientsHolderView)
        {
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
                Toasts.ShowToast("Empty box");
                return;
            }
            if (!activeBox.IsOpened)
            {
                Toasts.ShowToast("Before unpack the box");
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
                Toasts.ShowToast("Full ingredient container");
            }
        }
    }
}
