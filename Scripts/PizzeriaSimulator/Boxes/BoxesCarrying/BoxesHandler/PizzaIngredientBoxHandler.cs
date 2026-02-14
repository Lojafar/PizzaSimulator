using Game.PizzeriaSimulator.Boxes.Item.PizzaIngredient;
using Game.PizzeriaSimulator.Interactions;
using Game.PizzeriaSimulator.Interactions.Interactor;
using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold;
using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold.Visual;
using UnityEngine;

namespace Game.PizzeriaSimulator.Boxes.Carry.Handler
{
    sealed class PizzaIngredientBoxHandler : IBoxesHandler
    {
        readonly BoxesCarrier boxesCarrier;
        readonly Interactor interactor;
        readonly PizzaIngredientsHolder ingredientsHolder;
        readonly PizzaIngredientsHolderViewBase ingredientsHolderView;
        readonly int interactMaskWhenCarry;
        CarriableBoxBase activeBox;
        IPizzaIngredientsBox activeIngredientsBox;
        public PizzaIngredientBoxHandler(BoxesCarrier _boxesCarrier, Interactor _interactor, PizzaIngredientsHolder _ingredientsHolder, PizzaIngredientsHolderViewBase _ingredientsHolderView)
        {
            boxesCarrier = _boxesCarrier;
            interactor = _interactor;
            ingredientsHolder = _ingredientsHolder;
            ingredientsHolderView = _ingredientsHolderView; 
            interactMaskWhenCarry = 1 << (int)InteractableType.TrashCan
                | 1 << (int)InteractableType.PizzaCreateTable
                | 1 << (int)InteractableType.BoxWithItems;
        }

        public void SetBox(CarriableBoxBase box)
        {
            if(box is IPizzaIngredientsBox pizzaIngredientBox)
            {
                activeBox = box;
                activeIngredientsBox = pizzaIngredientBox;
                interactor.SetInteractionMask(interactMaskWhenCarry);
                return;
            }
            activeBox = null;
            activeIngredientsBox = null;
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
            if (ingredientsHolder.TryAddIngredient(activeIngredientsBox.IngredientType, false))
            {
                PizzaIngredientBoxItemBase item = activeIngredientsBox.RemoveAndGetItem();
                item.SetTo(ingredientsHolderView.GetNextIngredientPos(activeIngredientsBox.IngredientType),
                   () => ingredientsHolderView.SetIngredientToContainer(activeIngredientsBox.IngredientType, item.gameObject));
            }
            else
            {
                boxesCarrier.OnDenyAction("Full ingredient container");
            }
        }
    }
}
