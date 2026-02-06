using Game.PizzeriaSimulator.Boxes.Item;
using Game.PizzeriaSimulator.Interactions;
using Game.PizzeriaSimulator.Interactions.Interactor;
using Game.PizzeriaSimulator.Pizzeria.Furniture.Placement.Manager;
using Game.PizzeriaSimulator.Pizzeria.Furniture.Placement;
using Game.PizzeriaSimulator.Player.Input;
using Game.Root.Utils;
using UnityEngine;

namespace Game.PizzeriaSimulator.Boxes.Carry.Handler
{
    sealed class FurnitureBoxHandler : IBoxesHandler
    {
        readonly Interactor interactor;
        readonly IPlayerInput playerInput;
        readonly PizzeriaFurnitureManager furnitureManager;
        readonly Camera mainCamera;
        readonly int interactMaskWhenCarry;
        readonly int placementRayMask;
        readonly Vector3 screenMid = new(0.5f, 0.5f, 0f);
        Vector3 targetFurniturePos;
        FurnitureBoxBase currentBox;
        FurnitureBoxItemBase currentBoxItem;
        FurniturePlaceArea currentPlaceArea;
        Bounds currentItemBounds;
        bool itemCanBePlaced;
        const float placeRayDist = 14f;
        const float posLerpMod = 11f;
        public FurnitureBoxHandler(Interactor _interactor, IPlayerInput _playerInput, PizzeriaFurnitureManager _furnitureManager)
        {
            interactor = _interactor;
            playerInput = _playerInput;
            furnitureManager = _furnitureManager;
            mainCamera = Camera.main;
            interactMaskWhenCarry = 1 << (int)InteractableType.TrashCan | 1 << (int)InteractableType.BoxWithItems;
            placementRayMask = int.MaxValue;
        }
        public void SetBox(CarriableBoxBase box)
        {
            if (box is FurnitureBoxBase furnitureBox)
            {
                currentBox = furnitureBox;
                interactor.SetInteractionMask(interactMaskWhenCarry);
                return;
            }
            currentBox = null;
        }
        public void OnBoxOpened(bool opened)
        {
            UpdatePlaceAreaActive();
        }
        void UpdatePlaceAreaActive()
        {
            if (currentPlaceArea == null || currentBoxItem == null) return;
            if (currentBox.IsOpened && currentBox.ItemsAmount > 0) currentPlaceArea.gameObject.SetActive(true);
            else currentPlaceArea.gameObject.SetActive(false);
        }
        public void StartUsing()
        {
            if (currentBox == null) return;
            currentBoxItem = currentBox.GetFurnitureItem();
            if (currentBoxItem == null) return;

            targetFurniturePos = currentBoxItem.transform.position;
            itemCanBePlaced = false;
            currentBoxItem.SetAsCantBePlaced();
            currentPlaceArea = furnitureManager.GetPlaceAreaForItem(currentBox.FurnitureID);
            UpdatePlaceAreaActive();
            bool itemWasActive = currentBoxItem.gameObject.activeInHierarchy;
            currentBoxItem.gameObject.SetActive(true);
            currentItemBounds.extents = currentBoxItem.HalfSize;
            currentBoxItem.gameObject.SetActive(itemWasActive);

            Ticks.Instance.OnTick += UpdateBoxItemPos;
            Ticks.Instance.OnFixedTick += CheckRayForNewPos;
            playerInput.OnInteractInput += HandlePlaceInput;
        }
        public void EndUsing()
        {
            if (currentPlaceArea != null) currentPlaceArea.gameObject.SetActive(false);
            currentBoxItem = null;
            currentBox = null;
            currentPlaceArea = null;
            Ticks.Instance.OnTick -= UpdateBoxItemPos;
            Ticks.Instance.OnFixedTick -= CheckRayForNewPos;
            playerInput.OnInteractInput -= HandlePlaceInput;
        }
        void UpdateBoxItemPos()
        {
            if (currentBoxItem == null || !currentBox.IsOpened) return;
            currentBoxItem.SetPos(Vector3.Lerp(currentBoxItem.transform.position, targetFurniturePos, Time.deltaTime * posLerpMod));
        }
        void CheckRayForNewPos()
        {
            if (currentBoxItem == null || !currentBox.IsOpened) return;
            if (Physics.Raycast(mainCamera.ViewportPointToRay(screenMid), out RaycastHit hit, placeRayDist, placementRayMask, QueryTriggerInteraction.Ignore))
            {
                UpdateTargetPos(hit.point);
                CheckItemPlaceAcces();
            }
        }
        void UpdateTargetPos(Vector3 possiblePos)
        {
            currentItemBounds.center = possiblePos;
            if (!currentPlaceArea.IsBoundsInside(currentItemBounds)) currentPlaceArea.TryGetMagnetizedPos(currentItemBounds, out possiblePos);
            targetFurniturePos = possiblePos;
        }
        void CheckItemPlaceAcces()
        {
            currentItemBounds.center = currentBoxItem.transform.position;
            bool itemPlaceAcces = !currentBoxItem.CheckWallsCollision() && currentPlaceArea.IsBoundsInside(currentItemBounds);
            if (itemCanBePlaced != itemPlaceAcces)
            {
                itemCanBePlaced = itemPlaceAcces;
                if (itemCanBePlaced)
                {
                    playerInput.SelectInteractInput();
                    currentBoxItem.SetAsCanBePlaced();
                }
                else
                {
                    playerInput.DeselectInteractInput();
                    currentBoxItem.SetAsCantBePlaced();
                }
            }
        }
        void HandlePlaceInput()
        {
            if (!itemCanBePlaced || !currentBox.IsOpened || currentBox.ItemsAmount < 0 || currentBoxItem == null) return;
            furnitureManager.PlaceFurniture(currentBox.FurnitureID, currentBoxItem.transform.position);
            currentBoxItem.SetAsCantBePlaced();
            currentBox.RemoveItem();
            playerInput.DeselectInteractInput();
            UpdatePlaceAreaActive();
            if (currentBox.ItemsAmount < 1)
            {
                currentBoxItem = null;
                itemCanBePlaced = false;
            }
        }
       
    }
}
