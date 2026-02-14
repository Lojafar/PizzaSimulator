using Game.PizzeriaSimulator.Boxes.Item.Consumable;
using Game.PizzeriaSimulator.Interactions;
using Game.PizzeriaSimulator.Interactions.Interactor;
using Game.PizzeriaSimulator.SodaMachine;
using Game.PizzeriaSimulator.SodaMachine.Visual;
using UnityEngine;
using Zenject;

namespace Game.PizzeriaSimulator.Boxes.Carry.Handler
{
    sealed class ConsumablesBoxHandler : IBoxesHandler
    {
        readonly BoxesCarrier boxesCarrier;
        readonly Interactor interactor;
        readonly DiContainer diContainer;
        PizzeriaSodaMachine sodaMachine;
        PizzeriaSodaMachineViewBase sodaMachineView;
        CarriableBoxBase activeBox;
        IConsumablesBox activeConsumablesBox;
        public ConsumablesBoxHandler(BoxesCarrier _boxesCarrier, Interactor _interactor, DiContainer _diContainer)
        {
            boxesCarrier = _boxesCarrier;
            interactor = _interactor;
            diContainer = _diContainer;
        }

        public void SetBox(CarriableBoxBase box)
        {
            if (box is IConsumablesBox consumablesBox)
            {
                activeBox = box;
                activeConsumablesBox = consumablesBox;
                interactor.SetInteractionMask(ConsumTypeToInteractMask(activeConsumablesBox.ConsumableItemType));
                return;
            }
            activeBox = null;
            activeConsumablesBox = null;
        }
        int ConsumTypeToInteractMask(ConsumableBoxItemType consumableType)
        {
            return consumableType switch
            {
                ConsumableBoxItemType.SodaCup => 1 << (int)InteractableType.SodaMachine
                | 1 << (int)InteractableType.TrashCan
                | 1 << (int)InteractableType.BoxWithItems,
                _ => 1
            };
          
        }
        public void HandleInteraction(InteractableType interactableType, GameObject interactedObject)
        {
            if (interactableType == InteractableType.SodaMachine)
            {
                OnSodaMachineInteract(interactedObject);
            }
        }
        void OnSodaMachineInteract(GameObject interactedObject)
        {
            if(sodaMachine == null)
            {
                sodaMachine = diContainer.TryResolve<PizzeriaSodaMachine>();
                if (sodaMachine == null) return;
            }
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
            if (sodaMachine.TryAddEmptyCup())
            {
                if (sodaMachineView == null) sodaMachineView = interactedObject.GetComponent<PizzeriaSodaMachineViewBase>();
                ConsumableBoxItemBase item = activeConsumablesBox.RemoveAndGetItem();
                if (sodaMachineView != null && 
                    item.gameObject.TryGetComponent<SodaCupObject>(out SodaCupObject sodaCup))
                {
                    sodaMachineView.PrepareForExternalCup();
                    item.transform.parent = null;
                    (Vector3 cupPos, Vector3 cupRot) = sodaMachineView.GetNextCupPoint();
                    item.SetTo(cupPos, cupRot,
                       () => sodaMachineView.AddCupExternal(sodaCup));
                }
                else
                {
                    Object.Destroy(item.gameObject);
                }
            }
            else
            {
                boxesCarrier.OnDenyAction("Full ingredient container");
            }
        }
    }
}
