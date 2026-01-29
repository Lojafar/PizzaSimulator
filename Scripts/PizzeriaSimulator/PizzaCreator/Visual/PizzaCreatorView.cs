using Game.PizzeriaSimulator.PizzaCreation.Visual.Baker;
using Game.PizzeriaSimulator.PizzaCreation.Visual.IngredientOnPizza;
using Game.PizzeriaSimulator.PizzaCreation.IngredientsHold.Visual;
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
using Cysharp.Threading.Tasks;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual
{
    class PizzaCreatorView : PizzaCreatorViewBase
    {
        [SerializeField] AudioClip placeIngredientSFX;
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
        [SerializeField] float pizzaPrebakeJumpForce;
        DiContainer diContainer;
        Camera mainCam;
        PlayerCameraControllerBase playerCamController;

        Transform currentDragTransform;
        PizzaIngredientContainerBase currentContainer;
        PizzaObject currentPizza;
        RaycastHit containerHit;
        int containersRayLayerMask;
        Vector3 dragPosBeforeEnd;
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
            viewModel.ForceCurrentPizzaToBake += ForceCurrentPizzaInBake;
            viewModel.ForceIngredientPlace += ForceIngredientPlace;
            viewModel.ConfirmIngredientPlace += ConfirmPlaceDragged;
            viewModel.CancelIngredientPlace += CancellPlaceDragged;
            viewModel.ConfirmIngredientInput += ConfirmIngredientDrag;
            viewModel.DehighlightContainers += DehiglightAllContainers;
            viewModel.HighlightContainer += HiglightContainer;
            viewModel.ActivateBakeInput += SwitchInteractableBake;
            viewModel.BakePizza += BakePizza;
            viewModel.CancellPizzaBake += CancellPizzaBake;
            viewModel.PizzaAssembled += OnPizzaAssembled;
            HideOpenVisuals().Forget();
        }
        async UniTaskVoid HideOpenVisuals()
        {
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            objectWhenCreateOpen.SetActive(false);
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
                viewModel.ForceCurrentPizzaToBake -= ForceCurrentPizzaInBake;
                viewModel.ForceIngredientPlace -= ForceIngredientPlace;
                viewModel.ConfirmIngredientPlace -= ConfirmPlaceDragged;
                viewModel.CancelIngredientPlace -= CancellPlaceDragged;
                viewModel.ConfirmIngredientInput -= ConfirmIngredientDrag;
                viewModel.DehighlightContainers -= DehiglightAllContainers;
                viewModel.HighlightContainer -= HiglightContainer;
                viewModel.ActivateBakeInput -= SwitchInteractableBake;
                viewModel.BakePizza -= BakePizza;
                viewModel.CancellPizzaBake -= CancellPizzaBake;
                viewModel.PizzaAssembled -= OnPizzaAssembled;
            }
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
            Ticks.Instance.OnTick += OnUpdateWhenOpen;
        }
        void OnLeaveConstructor()
        {
            playerCamController.ResetLook(); 
            objectWhenCreateOpen.SetActive(false);
            Ticks.Instance.OnTick -= OnUpdateWhenOpen;
        }
        void OnUpdateWhenOpen()
        {
            UpdateDragObject();
            if (Input.GetMouseButtonDown(0))
            {
                if (CheckRayForContainer() && containerHit.collider.TryGetComponent<PizzaIngredientContainerBase>(out PizzaIngredientContainerBase container))
                {
                    currentContainer = container;
                    viewModel.IngredientInput.OnNext(currentContainer.IngredientType);
                }
                else
                {
                    currentContainer = null;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (currentDragTransform != null)
                {
                    dragPosBeforeEnd = currentContainer.GetDragIngredientPos();
                    currentContainer.EndDragObject();
                    currentDragTransform = null;
                    if (CheckRayForContainer() && containerHit.collider.gameObject == pizzaHolder.gameObject)
                    {
                        viewModel.IngredientPlaceInput.OnNext(currentContainer.IngredientType);
                    }
                    currentContainer = null;
                }
            }
        }
        bool CheckRayForContainer()
        {
            return Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out containerHit, maxContainersRayDist, containersRayLayerMask);
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
        void ForceCurrentPizzaInBake(BakedPizzaObject bakedPizzaPrefab, Action onBaked)
        {
            if (currentPizza == null) return;
            pizzaBaker.ForcePizzaBake(currentPizza, bakedPizzaPrefab, onBaked);
        }
        void ForceIngredientPlace(IngredientOnPizzaObjectBase onPizzaPrefab)
        {
            if (currentPizza == null) return;
            pizzaHolder.SpawnAndAddIngredient(onPizzaPrefab, dragPosBeforeEnd, currentPizza, true);
        }
        void ConfirmPlaceDragged(IngredientOnPizzaObjectBase onPizzaPrefab)
        {
            if (currentContainer != null && currentPizza != null)
            {
                pizzaHolder.SpawnAndAddIngredient(onPizzaPrefab, dragPosBeforeEnd, currentPizza);
                AudioPlayer.PlaySFX(placeIngredientSFX);
            }
        }
        void CancellPlaceDragged(string message)
        {
            ShowMessage(message);
            AudioPlayer.PlaySFX("Wrong");
        }
        void ShowMessage(string message)
        {
            Toasts.ShowToast(message);
        }
        void ConfirmIngredientDrag()
        {
            if(currentContainer != null)
            {
                currentDragTransform = currentContainer.GetObjectForDrag().transform;
            }
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
            AudioPlayer.PlaySFX("Swoosh"); 
            currentPizza.transform.DOJump(pizzaBaker.StartPizzaPosition, pizzaPrebakeJumpForce, 1, pizzaMovePrebakeDur).SetEase(Ease.Linear)
                .OnComplete(() => pizzaBaker.StartBaking(currentPizza, bakedPizzaPrefab, onBaked)).Play();
        }
        void CancellPizzaBake(string message)
        {
            ShowMessage(message);
            AudioPlayer.PlaySFX("Wrong");
        }
        void OnPizzaAssembled(string pizzaName)
        {
            ShowMessage(pizzaName + " assembled!");
        }
    }
}
