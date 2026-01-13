using Game.Helps;
using Game.PizzeriaSimulator.Player.CameraController;
using Game.Root.Utils;
using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using Game.Root.Utils.Audio;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual
{
    [Serializable]
    class PizzaCutSetting
    {
        [field: SerializeField] public int[] SlicesToMove { get; private set; }
        [field: SerializeField] public Vector3 MoveVector { get; private set; }
        [field: SerializeField] public Vector3 StartKnifeLocalPos { get; private set; }
        [field: SerializeField] public Vector3 EndKnifeLocalPos { get; private set; }
        [field: SerializeField] public Vector3 KnifeLocalRot { get; private set; }
    }
    class PizzaCutView : PizzaCutViewBase
    {
        [SerializeField] AudioClip pizzaCutSFX;
        [SerializeField] AudioClip pizzaMoveToBoxSFX;
        [SerializeField] PizzaCutSetting[] pizzaCutSettings;
        [SerializeField] Transform playerCamLookTransform;
        [SerializeField] Transform pizzaCutterTransform;
        [SerializeField] Transform pizzaBoxTopTransform;
        [SerializeField] Vector3 preBoxPizzaPos;
        [SerializeField] Vector3 boxPizzaPos;
        [SerializeField] Vector3 pizzaBoxTopClosedLocRot;
        [SerializeField] Button leaveButton;
        [SerializeField] Collider cutterCollider;
        [SerializeField] GameObject objectWhenCutOpen;
        [SerializeField] GameObject[] cutSelections;
        [SerializeField] Outline pizzaKnifeOutline;
        [SerializeField] float dragPosY;
        [SerializeField] float cutDuration;
        [SerializeField] float preBoxDelay;
        [SerializeField] float moveToBoxDuration;
        [SerializeField] float pizzaBoxTopRotDuration;
        [SerializeField] float pizzaBoxCloseDelay;
        DiContainer diContainer;
        Camera mainCam;
        PlayerCameraControllerBase playerCamController;
        Vector3 pizzaCutterNormalPos;
        Vector3 pizzaCutterNormalRot;
        Vector3 pizzaBoxTopNormaLocRot;
        RaycastHit rayHit;
        bool isDraggingCutter;
        bool isCuttingPizza;
        bool opened;
        int rayLayerMask;
        int currentCutState;
        int maxCutState = 3;
        Tween pizzaBoxAnim;
        const float maxrayHitDist = 3f;
        [Inject]
        public void Construct(DiContainer _diContainer)
        {
            diContainer = _diContainer;
        }
        public override void Bind(PizzaCutVM _viewModel)
        {
            objectWhenCutOpen.SetActive(false);
            maxCutState = pizzaCutSettings.Length;
            rayLayerMask = LayerMask.GetMask(Layers.DefaultLayerName);
            pizzaCutterNormalPos = pizzaCutterTransform.position;
            pizzaCutterNormalRot = pizzaCutterTransform.eulerAngles;
            pizzaBoxTopNormaLocRot = pizzaBoxTopTransform.localEulerAngles;
            mainCam = Camera.main;
            playerCamController = diContainer.Resolve<PlayerCameraControllerBase>();
            base.Bind(_viewModel);
            leaveButton.onClick.AddListener(OnLeaveBtn);
            viewModel.EnterPizzaCut += OnEnter;
            viewModel.LeavePizzaCut += OnLeave;

            pizzaBoxAnim = DOTween.Sequence()
                .Append(pizzaBoxTopTransform.DOLocalRotate(pizzaBoxTopClosedLocRot, pizzaBoxTopRotDuration).SetEase(Ease.Linear).OnComplete(FinishPizza))
                 .Append(pizzaBoxTopTransform.DOLocalRotate(pizzaBoxTopClosedLocRot, pizzaBoxCloseDelay).SetEase(Ease.Linear))
                 .Append(pizzaBoxTopTransform.DOLocalRotate(pizzaBoxTopNormaLocRot, pizzaBoxTopRotDuration).SetEase(Ease.Linear)).SetAutoKill(false);
        }
        void FinishPizza()
        {
            Destroy(currentPizzaToCut.gameObject);
            currentPizzaToCut = null;
            viewModel.PizzaCuttedInput.OnNext(Unit.Default);
        }
        private void OnDestroy()
        {
            leaveButton.onClick.RemoveListener(OnLeaveBtn);
            if (viewModel != null)
            {
                viewModel.EnterPizzaCut -= OnEnter;
                viewModel.LeavePizzaCut -= OnLeave;
            }
            Ticks.Instance.OnTick -= OnUpdateWhenOpen;
        }
        void OnLeaveBtn()
        {
            viewModel.LeaveInput.OnNext(Unit.Default);
        }
        void OnEnter()
        {
            opened = true;
            if (currentPizzaToCut != null) pizzaKnifeOutline.enabled = true;
            playerCamController.SetLook(playerCamLookTransform.position, playerCamLookTransform.eulerAngles);
            objectWhenCutOpen.SetActive(true);
            Ticks.Instance.OnTick += OnUpdateWhenOpen;
        }
        void OnLeave()
        {
            opened = false;
            if(!isCuttingPizza) ResetCutter();
            isDraggingCutter = false;
            pizzaKnifeOutline.enabled = false;
            playerCamController.ResetLook();
            objectWhenCutOpen.SetActive(false);
            Ticks.Instance.OnTick -= OnUpdateWhenOpen;
        }
        void ResetCutter()
        {
            cutterCollider.enabled = true;
            pizzaCutterTransform.position = pizzaCutterNormalPos;
            pizzaCutterTransform.eulerAngles = pizzaCutterNormalRot;
        }
        void OnUpdateWhenOpen()
        {
            UpdateDragObject();
            if (Input.GetMouseButtonDown(0) && !isDraggingCutter)
            {
                if (Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out rayHit, maxrayHitDist, rayLayerMask))
                {
                    if (rayHit.transform == pizzaCutterTransform)
                    {
                        cutterCollider.enabled = false;
                        isDraggingCutter = true;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (isDraggingCutter && currentPizzaToCut != null && currentCutState < maxCutState
                    && Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out rayHit, maxrayHitDist, rayLayerMask) && rayHit.collider.gameObject == currentPizzaToCut.gameObject)
                {
                    CutPizza();
                }
                else
                {
                    ResetCutter();
                }
                isDraggingCutter = false;
            }
        }
        void UpdateDragObject()
        {
            if (!isDraggingCutter) return;
            Vector3 xzDragPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f));
            pizzaCutterTransform.position = new Vector3(xzDragPos.x, dragPosY, xzDragPos.z);
        }
        void CutPizza()
        {
            AudioPlayer.PlaySFX(pizzaCutSFX);
            isCuttingPizza = true;
            pizzaCutterTransform.localEulerAngles = pizzaCutSettings[currentCutState].KnifeLocalRot;
            pizzaCutterTransform.localPosition = pizzaCutSettings[currentCutState].StartKnifeLocalPos;
            pizzaCutterTransform.DOLocalMove(pizzaCutSettings[currentCutState].EndKnifeLocalPos, cutDuration).SetEase(Ease.Linear)
                .OnComplete(() => FinishCutState()).Play();
        }
        void FinishCutState()
        {
            foreach (int sliceID in pizzaCutSettings[currentCutState].SlicesToMove)
            {
                currentPizzaToCut.GetPizzaSlice(sliceID).transform.position += pizzaCutSettings[currentCutState].MoveVector;
            }
            isCuttingPizza = false;
            ResetCutter();
            currentCutState++;
            UpdatePizzaCutSelection();
            if (currentCutState == maxCutState)
            { 
                pizzaKnifeOutline.enabled = false;
                TranslatePizzaToBox(); 
            }
        }
        async void TranslatePizzaToBox()
        {
            await UniTask.WaitForSeconds(preBoxDelay);
            AudioPlayer.PlaySFX(pizzaMoveToBoxSFX);
            DOTween.Sequence()
                .Append(currentPizzaToCut.transform.DOMove(preBoxPizzaPos, moveToBoxDuration).SetEase(Ease.Linear))
                .Append(currentPizzaToCut.transform.DOMove(boxPizzaPos, moveToBoxDuration).SetEase(Ease.Linear))
                .Play().OnComplete(PizzaFinishedBox);
        }
        void PizzaFinishedBox()
        {
            pizzaBoxAnim.Restart();
            pizzaBoxAnim.Play();
        }
        public override void SetPizzaToCut(BakedPizzaObject pizzaObject)
        {
            if(pizzaObject == null) return;
            base.SetPizzaToCut(pizzaObject);
            currentCutState = 0;
            if(opened) pizzaKnifeOutline.enabled = true;
            UpdatePizzaCutSelection();
        }
        void UpdatePizzaCutSelection()
        {
            if (cutSelections.Length < maxCutState) return;
            for(int i = 0; i < maxCutState; i++)
            {
                if (i < currentCutState)
                {
                    cutSelections[i].SetActive(false);
                }
                else
                {
                    cutSelections[i].SetActive(true);
                }
            }
        }
    }
}
