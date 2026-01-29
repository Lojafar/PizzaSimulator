using Game.Root.Utils;
using Game.Root.ServicesInterfaces;
using Game.Helps;
using Game.PizzeriaSimulator.Player.CameraController;
using Game.PizzeriaSimulator.Player.Input;
using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Interactions.Interactor
{
    public class Interactor : IInittable, ISceneDisposable
    {
        public int InitPriority => 9;
        public event Action<InteractableType> OnInteract;
        public event Action<InteractableType, GameObject> OnInteractWithObject;
        readonly IPlayerInput playerInput;
        readonly PlayerCameraControllerBase cameraController;
        readonly Vector3 midOfScreen;
        readonly int interactLayerMask;
        RaycastHit interactHit;
        Camera mainCam;

        bool camLocked;
        Interactable lastInteractable;
        const float maxInteractDist = 4f;
        public Interactor(IPlayerInput _playerInput, PlayerCameraControllerBase _cameraController)
        {
            playerInput = _playerInput;
            cameraController = _cameraController;
            midOfScreen = new Vector3(0.5f, 0.5f, 0);
            interactLayerMask = LayerMask.GetMask(Layers.InteractableLayerName, Layers.ObstaclesLayerName);
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
                if (interactable != lastInteractable)
                {
                    if (lastInteractable != null) lastInteractable.Deselect();
                    playerInput.SelectInteractInput();
                    lastInteractable = interactable;
                    lastInteractable.Select();
                }
            }
            else if (lastInteractable != null)
            {
                playerInput.DeselectInteractInput();
                lastInteractable.Deselect();
                lastInteractable = null;
            }
        }
        void OnInteractInput()
        {
            if (lastInteractable == null || camLocked) return;
            OnInteract?.Invoke(lastInteractable.InteractableType);
            OnInteractWithObject?.Invoke(lastInteractable.InteractableType, lastInteractable.gameObject);
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