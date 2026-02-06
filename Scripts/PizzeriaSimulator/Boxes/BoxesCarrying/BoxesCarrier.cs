using Game.Root.ServicesInterfaces;
using Game.PizzeriaSimulator.Boxes.Carry.Handler;
using Game.PizzeriaSimulator.Interactions;
using Game.PizzeriaSimulator.Interactions.Interactor;
using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold;
using Game.PizzeriaSimulator.Player.CameraController;
using Game.PizzeriaSimulator.Player.Input;
using Game.PizzeriaSimulator.Pizzeria.Furniture.Placement.Manager;
using System.Collections.Generic;
using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Game.PizzeriaSimulator.Boxes.Carry
{
    using Object = UnityEngine.Object;
    public class BoxesCarrier : IInittable, ISceneDisposable
    {
        public int InitPriority => 8;
        public bool IsCarryingBox { get; private set; }
        public event Action<uint> OnBoxPicked;
        public event Action<uint> OnBoxThrowed;
        public event Action<uint> OnBoxRemoved;
        public event Action<string> OnActionDenied;
        readonly Interactor interactor;
        readonly IPlayerInput playerInput;
        readonly Transform playerItemContainer;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly DiContainer diContainer;
        readonly Dictionary<CarriableBoxType, IBoxesHandler> boxHandlerByType;
        IBoxesHandler activeBoxHandler;
        CarriableBoxBase activeBox;
        GameObject activeBoxObject;
        const float throwForce = 4f;
        const float boxToTrashPower = 0.5f;
        const float boxToTrashDuration = 0.3f;
        public BoxesCarrier(Interactor _interactor, IPlayerInput _playerInput, PlayerCameraControllerBase _playerCameraController,
            PizzeriaSceneReferences _sceneReferences, DiContainer _diContainer)
        {
            interactor = _interactor;
            playerInput = _playerInput;
            playerItemContainer = _playerCameraController.InHandsTransform;
            sceneReferences = _sceneReferences;
            diContainer = _diContainer;
            boxHandlerByType = new Dictionary<CarriableBoxType, IBoxesHandler>();
        }
        public void Init()
        {
            boxHandlerByType.Add(CarriableBoxType.PizzaIngredientsBox, new PizzaIngredientBoxHandler(this, interactor, diContainer.Resolve<PizzaIngredientsHolder>(),
                sceneReferences.PizzaIngredientsHoldView));
            boxHandlerByType.Add(CarriableBoxType.FurnitureBox, new FurnitureBoxHandler(interactor, playerInput, diContainer.Resolve<PizzeriaFurnitureManager>()));
            interactor.OnInteractWithObject += HandleInteractor;
            playerInput.OnThrowInput += HandleThrowInput;
            playerInput.OnOpenInput += HandleOpenBoxInput;
        }
        public void Dispose()
        {
            activeBoxHandler?.EndUsing();
            interactor.OnInteractWithObject -= HandleInteractor;
            playerInput.OnThrowInput -= HandleThrowInput;
            playerInput.OnOpenInput -= HandleOpenBoxInput;
        }
        void HandleInteractor(InteractableType interactableType, GameObject interactedObject)
        {
            switch (interactableType)
            {
                case InteractableType.BoxWithItems:
                    HandleThrowInput();
                    HandlePossibleBox(interactedObject);
                    break;
                case InteractableType.TrashCan:
                    HandleTrashCanInteract(interactedObject); 
                    break;
            }
            activeBoxHandler?.HandleInteraction(interactableType, interactedObject);
        }
        public void HandlePossibleBox(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out CarriableBoxBase box))
            {
                if (boxHandlerByType.TryGetValue(box.BoxType, out IBoxesHandler boxesHandler))
                {
                    boxesHandler.SetBox(box);
                    activeBoxHandler = boxesHandler;
                    activeBoxHandler.StartUsing();
                }
                activeBox = box;
                IsCarryingBox = true;
                activeBox.OnPicked();
                activeBoxObject = gameObject;
                activeBoxObject.transform.parent = playerItemContainer;
                activeBoxObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                playerInput.ShowThrowInput(true);
                OnBoxPicked?.Invoke(activeBox.BoxObjectID);
                if (activeBox.IsOpened)
                {
                    playerInput.ShowOpenInput(false);
                    playerInput.ShowCloseInput(true);
                }
                else
                {
                    playerInput.ShowOpenInput(true);
                    playerInput.ShowCloseInput(false);
                }
            }
        }
      
        void HandleTrashCanInteract(GameObject trashCanObject)
        {
            if (!IsCarryingBox) return;
            if (activeBox.ItemsAmount > 0)
            {
                OnDenyAction("Active box has items inside. Can't throw to the can");
                return;
            }
            OnEndCarry();
            GameObject boxObject = activeBoxObject;
            OnBoxRemoved?.Invoke(activeBox.BoxObjectID);
            activeBoxObject.transform.DOJump(trashCanObject.transform.position, boxToTrashPower, 1, boxToTrashDuration)
                .SetEase(Ease.Linear).OnComplete(() => Object.Destroy(boxObject)).Play();
            ClearBoxValues();
        }
        void HandleThrowInput()
        {
            if (activeBox == null || activeBoxObject == null) return;
            activeBox.Throw(activeBoxObject.transform.forward * throwForce);
            OnEndCarry();
             OnBoxThrowed?.Invoke(activeBox.BoxObjectID);
            ClearBoxValues();

        }
        void OnEndCarry()
        {
            interactor.ResetInteractionMask();
            playerInput.ShowThrowInput(false);
            playerInput.ShowOpenInput(false);
            playerInput.ShowCloseInput(false);
        }
        void ClearBoxValues()
        {
            if (activeBoxObject != null) 
            {
                activeBoxObject.transform.parent = null;
            }
            IsCarryingBox = false;
            activeBox = null;
            activeBoxObject = null;
            if (activeBoxHandler != null)
            {
                activeBoxHandler.EndUsing();
                activeBoxHandler = null;
            }
        }
        void HandleOpenBoxInput()
        {
            if (!IsCarryingBox) return;
            if (activeBox.IsOpened)
            {
                activeBox.Close();
                playerInput.ShowOpenInput(true);
                playerInput.ShowCloseInput(false);
            }
            else
            {
                activeBox.Open();
                playerInput.ShowOpenInput(false);
                playerInput.ShowCloseInput(true);
            }
             activeBoxHandler?.OnBoxOpened(activeBox.IsOpened);
        }
        public void OnDenyAction(string message)
        {
            OnActionDenied?.Invoke(message);
        }
    }
}
