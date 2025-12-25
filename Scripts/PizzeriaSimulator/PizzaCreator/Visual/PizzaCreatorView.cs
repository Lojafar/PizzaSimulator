using Game.PizzeriaSimulator.PizzaCreation.Visual.Baker;
using Game.PizzeriaSimulator.PizzaCreation.Visual.IngredientOnPizza;
using Game.PizzeriaSimulator.Player.CameraController;
using Game.Root.Utils;
using Game.Helps;
using System;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using Game.Root.Utils.Audio;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual
{
    class PizzaCreatorView : PizzaCreatorViewBase
    {
        [SerializeField] AudioClip placeIngredientSFX;
        [SerializeField] AudioClip cancelSFX;
        [SerializeField] AudioClip confirmPizzaSFX;
        [SerializeField] AudioClip clearPizzaSFX;
        [SerializeField] Transform playerCamLookTransform;
        [SerializeField] Transform pizzaPreBakePoint;
        [SerializeField] CreatedPizzaHolder pizzaHolder;
        [SerializeField] PizzaBakerObject pizzaBaker;
        [SerializeField] Button leaveButton;
        [SerializeField] Button clearPizzaButton;
        [SerializeField] Button bakeButton;
        [SerializeField] Outline[] containersOutlines;
        [SerializeField] GameObject objectWhenCreateOpen;
        [SerializeField] float dragPosY;
        [SerializeField] float pizzaMovePrebakeDur;
        DiContainer diContainer;
        Camera mainCam;
        PlayerCameraControllerBase playerCamController;

        Transform currentDragTransform;
        PizzaIngredientContainerBase currentContainer;
        PizzaObject currentPizza;
        RaycastHit containersHit;
        int containersRayLayerMask;
        const float maxContainersRayDist = 3f;
        [Inject]
        public void Construct(DiContainer _diContainer)
        {
            diContainer = _diContainer;
        }
        public override void Bind(PizzaCreatorVM _viewModel)
        {
            containersRayLayerMask = LayerMask.GetMask(Layers.DefaultLayerName);
            mainCam = Camera.main;
            playerCamController = diContainer.Resolve<PlayerCameraControllerBase>();
            base.Bind(_viewModel);
            leaveButton.onClick.AddListener(OnLeaveBtn);
            clearPizzaButton.onClick.AddListener(OnClearPizzaBtn);
            bakeButton.onClick.AddListener(OnBakeBtn);
            viewModel.EnterConstruction += OnEnterConstructor;
            viewModel.LeaveConstruction += OnLeaveConstructor;
            viewModel.OnPizzaStarted += CreatePizzaObject;
            viewModel.RemovePizza += RemovePizza;
            viewModel.ConfirmIngredientPlace += ConfirmPlaceDragged;
            viewModel.CancelIngredientPlace += CancellPlaceDragged;
            viewModel.DehighlightContainers += DehiglightAllContainers;
            viewModel.HighlightContainer += HiglightContainer;
            viewModel.ActivateBakeInput += SwitchInteractableBake;
            viewModel.BakePizza += BakePizza;
            viewModel.CancellPizzaBake += CancellPizzaBake;
            viewModel.PizzaAssembled += OnPizzaAssembled;
        }
        private void OnDestroy()
        {
            leaveButton.onClick.RemoveListener(OnLeaveBtn);
            clearPizzaButton.onClick.RemoveListener(OnClearPizzaBtn);
            bakeButton.onClick.RemoveListener(OnBakeBtn);
            if (viewModel != null)
            {
                viewModel.EnterConstruction -= OnEnterConstructor;
                viewModel.LeaveConstruction -= OnLeaveConstructor;
                viewModel.OnPizzaStarted -= CreatePizzaObject;
                viewModel.RemovePizza -= RemovePizza;
                viewModel.ConfirmIngredientPlace -= ConfirmPlaceDragged;
                viewModel.CancelIngredientPlace -= CancellPlaceDragged;
                viewModel.DehighlightContainers -= DehiglightAllContainers;
                viewModel.HighlightContainer -= HiglightContainer;
                viewModel.ActivateBakeInput -= SwitchInteractableBake;
                viewModel.BakePizza -= BakePizza;
                viewModel.CancellPizzaBake -= CancellPizzaBake;
                viewModel.PizzaAssembled -= OnPizzaAssembled;
            }
            Ticks.Instance.OnFixedTick -= OnFixedUpdateWhenOpen;
            Ticks.Instance.OnTick -= OnUpdateWhenOpen;
        }
        void OnLeaveBtn()
        {
            viewModel.LeaveInput.OnNext(Unit.Default);
        }
        void OnClearPizzaBtn()
        {
            viewModel.ClearPizzaInput.OnNext(Unit.Default);
        }
        void OnBakeBtn()
        {
            viewModel.BakeInput.OnNext(Unit.Default);
        }
        void OnEnterConstructor()
        {
            playerCamController.SetLook(playerCamLookTransform.position, playerCamLookTransform.eulerAngles);
            objectWhenCreateOpen.SetActive(true);
            Ticks.Instance.OnFixedTick += OnFixedUpdateWhenOpen;
            Ticks.Instance.OnTick += OnUpdateWhenOpen;
        }
        void OnLeaveConstructor()
        {
            playerCamController.ResetLook(); 
            objectWhenCreateOpen.SetActive(false);
            Ticks.Instance.OnFixedTick -= OnFixedUpdateWhenOpen;
            Ticks.Instance.OnTick -= OnUpdateWhenOpen;
        }
        void OnFixedUpdateWhenOpen()
        {
            if (currentDragTransform != null) return;
            if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out containersHit, maxContainersRayDist, containersRayLayerMask))
            {
                if (currentContainer != null && currentContainer.gameObject == containersHit.collider.gameObject) return;
                if (containersHit.collider.TryGetComponent<PizzaIngredientContainerBase>(out PizzaIngredientContainerBase container))
                {
                    currentContainer = container;
                    return;
                }
            }
            else
            {
                currentContainer = null;
            }
        }  
        void OnUpdateWhenOpen()
        {
            if (currentContainer == null) return;
            UpdateDragObject();
            if (Input.GetMouseButtonDown(0))
            {
                currentDragTransform = currentContainer.GetObjectForDrag().transform;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (currentDragTransform != null)
                {
                    if(Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out containersHit, maxContainersRayDist, containersRayLayerMask)
                      && containersHit.collider.gameObject == pizzaHolder.gameObject)
                    {
                        viewModel.IngredientPlaceInput.OnNext(currentContainer.IngredientType);
                    }
                    else
                    {
                        currentContainer.CancelDragObject();
                    }
                    currentDragTransform = null;
                }
            }
        }
        void UpdateDragObject()
        {
            if (currentDragTransform == null) return;
            Vector3 xzDragPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f));
            currentDragTransform.position = new Vector3(xzDragPos.x, dragPosY, xzDragPos.z);
        }
        void CreatePizzaObject()
        {
            currentPizza = pizzaHolder.SpawnNewPizzaObject();
        }
        void RemovePizza()
        {
            AudioPlayer.PlaySFX(clearPizzaSFX);
            Destroy(currentPizza.gameObject);
            currentPizza = null;
        }
        void ConfirmPlaceDragged(IngredientOnPizzaObjectBase onPizzaPrefab)
        {
            if (currentContainer != null && currentDragTransform != null && currentPizza != null)
            {
                pizzaHolder.SpawnAndAddIngredient(onPizzaPrefab, currentContainer.GetDragIngridientPos(), currentPizza);
                currentContainer.RemoveDraggedObject();
                currentDragTransform = null;
                AudioPlayer.PlaySFX(placeIngredientSFX);
            }
        }
       
        void CancellPlaceDragged(string message)
        {
            ShowMessage(message);
            AudioPlayer.PlaySFX(cancelSFX);
            if (currentContainer != null && currentDragTransform != null)
            {
                currentContainer.CancelDragObject();
                currentDragTransform = null;
            }
        }
        void ShowMessage(string message)
        {
            Toasts.ShowToast(message);
        }
        void DehiglightAllContainers()
        {
            foreach (Outline outline in containersOutlines) outline.enabled = false;
        }
        void HiglightContainer(PizzaIngredientType ingredientType)
        {
            int i = (int)ingredientType; 
            if(containersOutlines.Length > i) containersOutlines[i].enabled = true;
        }
        void SwitchInteractableBake(bool active)
        {
            bakeButton.interactable = active;
        }
        void BakePizza(BakedPizzaObject bakedPizzaPrefab, Action onBaked)
        {
            AudioPlayer.PlaySFX(confirmPizzaSFX);
           DOTween.Sequence()
                .Append(currentPizza.transform.DOMove(pizzaPreBakePoint.position, pizzaMovePrebakeDur).SetEase(Ease.Linear))
                .Append(currentPizza.transform.DOMove(pizzaBaker.StartPizzaPosition, pizzaMovePrebakeDur).SetEase(Ease.Linear))
                .OnComplete(() => pizzaBaker.StartBaking(currentPizza, bakedPizzaPrefab, onBaked)).Play();
        }
        void CancellPizzaBake(string message)
        {
            ShowMessage(message);
            AudioPlayer.PlaySFX(cancelSFX);
        }
        void OnPizzaAssembled(string pizzaName)
        {
            ShowMessage(pizzaName + " assembled!");
        }
    }
}
