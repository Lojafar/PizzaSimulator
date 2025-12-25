using Game.Helps;
using Game.PizzeriaSimulator.OrdersHandle;
using Game.PizzeriaSimulator.PaymentReceive;
using Game.PizzeriaSimulator.PizzaCreation;
using Game.PizzeriaSimulator.Player.CameraController;
using Game.PizzeriaSimulator.Player.Input;
using Game.Root.ServicesInterfaces;
using Game.Root.Utils;
using UnityEngine;

namespace Game.PizzeriaSimulator.Interactions.Interactor
{
    public class Interactor : ISceneDisposable
    {
        readonly IPlayerInput playerInput;
        readonly PlayerCameraControllerBase cameraController;
        readonly PaymentReceiver paymentReceiver;
        readonly PizzaCreator pizzaCreator;
        readonly PizzeriaOrdersHandler ordersHandler;
        readonly Vector3 midOfScreen;
        readonly int interactLayerMask;
        RaycastHit interactHit;
        Camera mainCam;

        bool camLocked;
        Interactable lastInteractable;
        const float maxInteractDist = 4f;
        public Interactor(IPlayerInput _playerInput, PlayerCameraControllerBase _cameraController, PaymentReceiver _paymentReceiver, PizzaCreator _pizzaCreator, PizzeriaOrdersHandler _ordersHandler) 
        {
            playerInput = _playerInput;
            cameraController = _cameraController;
            paymentReceiver = _paymentReceiver;
            pizzaCreator = _pizzaCreator;
            ordersHandler = _ordersHandler;
            midOfScreen = new Vector3(0.5f, 0.5f, 0);
            interactLayerMask = LayerMask.GetMask(Layers.InteractableLayerName, Layers.DefaultLayerName);
        }
        public void Init()
        {
            mainCam = Camera.main;
            Ticks.Instance.OnFixedTick += OnFixedUpdate;
            playerInput.OnInteractInput += OnInteractInput;
            cameraController.OnCameraLocked += OnCamLocked;
        }
        public void Dispose()
        {
            Ticks.Instance.OnFixedTick -= OnFixedUpdate;
            playerInput.OnInteractInput -= OnInteractInput;
            cameraController.OnCameraLocked -= OnCamLocked;
        }
        void OnFixedUpdate()
        {
            if (camLocked) return;
                if (Physics.Raycast(mainCam.ViewportPointToRay(midOfScreen), out interactHit, maxInteractDist, interactLayerMask) &&
                interactHit.collider.TryGetComponent<Interactable>(out Interactable interactable))
            {
                if(interactable != lastInteractable)
                {
                    if(lastInteractable != null) lastInteractable.Deselect();
                    playerInput.SelectInteractInput();
                    lastInteractable = interactable;
                    lastInteractable.Select();
                }
            }
            else if(lastInteractable != null) 
            {
                playerInput.DeselectInteractInput();
                lastInteractable.Deselect();
                lastInteractable = null;
            }
        }
        void OnInteractInput()
        {
            if (lastInteractable == null || camLocked) return;
            switch (lastInteractable.InteractableType)
            {
                case InteractableType.PaymentStand:
                    paymentReceiver.EnterPaymentReceive();
                    break;
                case InteractableType.PizzaCreateTable:
                    pizzaCreator.EnterPizzaCreate();
                    break;
                case InteractableType.PizzaCutTable:
                    pizzaCreator.EnterPizzaCut();
                    break; 
                case InteractableType.OrderBell:
                    ordersHandler.CallBell();
                    break;
            }
            if (!lastInteractable.CycledInteract)
            {
                playerInput.DeselectInteractInput();
                lastInteractable.Deselect();
                lastInteractable = null;
            }
        }
        void OnCamLocked(bool isLocked)
        {
            camLocked = isLocked;
        }
    }
}
