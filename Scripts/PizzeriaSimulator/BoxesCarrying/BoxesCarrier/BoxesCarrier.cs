using Game.Root.ServicesInterfaces;
using Game.Root.Utils;
using Game.PizzeriaSimulator.BoxCarry.Carrier.Handler;
using Game.PizzeriaSimulator.BoxCarry.Box;
using Game.PizzeriaSimulator.Interactions;
using Game.PizzeriaSimulator.Interactions.Interactor;
using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold;
using Game.PizzeriaSimulator.Player.CameraController;
using Game.PizzeriaSimulator.Player.Input;
using DG.Tweening;
using UnityEngine;
using Zenject;
using System.Collections.Generic;

namespace Game.PizzeriaSimulator.BoxCarry.Carrier
{
    public class BoxesCarrier : IInittable, ISceneDisposable
    {
        public bool IsCarryingBox { get; private set; }
        readonly Interactor interactor;
        readonly IPlayerInput playerInput;
        readonly Transform playerItemContainer;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly DiContainer diContainer;
        readonly Dictionary<CarriableBoxType, IBoxesHandler> boxHandlerByType;
        IBoxesHandler activeBoxHandler;
        ICarriableBox activeBox;
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
            boxHandlerByType.Add(CarriableBoxType.PizzaIngredientsBox, new PizzaIngredientBoxHandler(diContainer.Resolve<PizzaIngredientsHolder>(),
                sceneReferences.PizzaIngredientsHoldView));
            interactor.OnInteractWithObject += HandleInteractor;
            playerInput.OnThrowInput += HandleThrowInput;
            playerInput.OnOpenInput += HandleOpenBoxInput;
        }
        public void Dispose()
        {
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
            if (gameObject.TryGetComponent(out ICarriableBox box))
            {
                if(boxHandlerByType.TryGetValue(box.BoxType, out IBoxesHandler boxesHandler))
                {
                    boxesHandler.SetBox(box);
                    activeBoxHandler = boxesHandler;
                }
                activeBox = box;
                IsCarryingBox = true;
                activeBox.OnPicked();
                activeBoxObject = gameObject;
                activeBoxObject.transform.parent = playerItemContainer;
                activeBoxObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }
        void HandleTrashCanInteract(GameObject trashCanObject)
        {
            if (!IsCarryingBox) return;
            if (activeBox.ItemsAmount > 0)
            {
                Toasts.ShowToast("Active box has items inside. Can't throw to the can");
                return;
            }
            GameObject boxObject = activeBoxObject;
            activeBoxObject.transform.DOJump(trashCanObject.transform.position, boxToTrashPower, 1, boxToTrashDuration)
                .SetEase(Ease.Linear).OnComplete(() => Object.Destroy(boxObject)).Play();
            ClearBoxValues();
        }
        void HandleThrowInput()
        {
            if (activeBox == null || activeBoxObject == null) return;
            activeBox.Throw(activeBoxObject.transform.forward * throwForce);
            ClearBoxValues();
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
            activeBoxHandler = null;
        }
        void HandleOpenBoxInput()
        {
            if (!IsCarryingBox) return;
            if (activeBox.IsOpened)
            {
                activeBox.Close();
            }
            else
            {
                activeBox.Open();
            }
        }
    }
}
