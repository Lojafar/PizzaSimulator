using Game.PizzeriaSimulator.BoxCarry.Box;
using Game.PizzeriaSimulator.BoxCarry.Box.Item;
using Game.PizzeriaSimulator.Interactions;
using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold;
using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold.Visual;
using Game.Root.Utils;
using UnityEngine;

namespace Game.PizzeriaSimulator.BoxCarry.Carrier.Handler
{
    class PizzaIngredientBoxHandler : IBoxesHandler
    {
        readonly PizzaIngredientsHolder ingredientsHolder;
        readonly PizzaIngredientsHolderViewBase ingredientsHolderView;
        IPizzaIngredientBox activeBox;
        public PizzaIngredientBoxHandler(PizzaIngredientsHolder _ingredientsHolder, PizzaIngredientsHolderViewBase _ingredientsHolderView)
        {
            ingredientsHolder = _ingredientsHolder;
            ingredientsHolderView = _ingredientsHolderView;
        }

        public void SetBox(ICarriableBox box)
        {
            if(box is IPizzaIngredientBox pizzaIngredientBox)
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
