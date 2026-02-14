using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.PizzeriaSimulator.Orders.ObjsContainer;
using Game.PizzeriaSimulator.Player.CameraController;
using Game.UI3D.Elements;
using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
namespace Game.PizzeriaSimulator.SodaMachine.Visual
{
    sealed class PizzeriaSodaMachineView : PizzeriaSodaMachineViewBase
    {
        [SerializeField] SodaCupObject cupPrefab;
        [SerializeField] Button leaveButton;
        [SerializeField] MeshButton fillCup1MBtn;
        [SerializeField] MeshButton fillCup2MBtn;
        [SerializeField] MeshButton fillCup3MBtn;
        [SerializeField] MeshButton fillCup4MBtn;
        [SerializeField] GameObject openVisuals;
        [SerializeField] GameObject[] sodaFountains;
        [SerializeField] MeshRenderer[] fillBtnsRenderers;
        [SerializeField] Transform camLookTransform;
        [SerializeField] Transform emptyCupsSection1;
        [SerializeField] Transform emptyCupsSection2;
        [SerializeField] Transform[] fillCupsPoints;
        [SerializeField] Material availableFillBtnMat;
        [SerializeField] Material notAvailableFillBtnMat;
        [SerializeField] Vector3 cupAfterFillLPosAdd;
        [SerializeField] float maxCupsPerSection;
        [SerializeField] float cupFillDuration;
        [SerializeField] float cupAfterFillMoveDur;
        [SerializeField] float newCupOffsetY;
        [SerializeField] float cupJumpToFillForce;
        [SerializeField] float cupJumpToFillDuration;
        readonly List<SodaCupObject> spawnedCups = new();
        readonly SodaCupObject[] cupsInFill = new SodaCupObject[PizzeriaSodaMachineVM.FillableSlotsCount];
        OrderItemsObjsContainer orderItemsContainer;
        DiContainer diContainer;
        PlayerCameraControllerBase playerCamController;
        int newCupsIgnors;
        [Inject]
        public void Construct(PizzeriaSceneReferences sceneReferences, DiContainer _diContainer)
        {
            orderItemsContainer = sceneReferences.RemovedOrderItemsContainer;
            diContainer = _diContainer;
        }
        void Awake()
        {
            openVisuals.SetActive(false);
            for (int i = 0; i < sodaFountains.Length; i++) 
            {
                sodaFountains[i].SetActive(false); 
            }
        }
        public override (Vector3, Vector3) GetNextCupPoint()
        {
            Transform section = (spawnedCups.Count >= maxCupsPerSection) ? emptyCupsSection2 : emptyCupsSection1;
            return (new Vector3(section.position.x,section.position.y, section.position.z) + section.up * CaclNextCupOffsetY(), section.eulerAngles);
        }
        float CaclNextCupOffsetY()
        {
            return newCupOffsetY * (spawnedCups.Count - ((spawnedCups.Count >= maxCupsPerSection) ? maxCupsPerSection : 0));
        }
        public override void PrepareForExternalCup()
        {
            newCupsIgnors++;
        }
        public override void AddCupExternal(SodaCupObject cupObject)
        {
            cupObject.transform.parent = (spawnedCups.Count >= maxCupsPerSection) ? emptyCupsSection2 : emptyCupsSection1;
            cupObject.transform.localPosition = new Vector3(0, CaclNextCupOffsetY(), 0);
            cupObject.transform.localEulerAngles = Vector3.zero;
            spawnedCups.Add(cupObject);
        }
        public override void Bind(PizzeriaSodaMachineVM _viewModel)
        {
            playerCamController = diContainer.Resolve<PlayerCameraControllerBase>();
            leaveButton.onClick.AddListener(OnLeaveBtn);
            fillCup1MBtn.OnClick.AddListener(OnFillCup1Btn);
            fillCup2MBtn.OnClick.AddListener(OnFillCup2Btn);
            fillCup3MBtn.OnClick.AddListener(OnFillCup3Btn);
            fillCup4MBtn.OnClick.AddListener(OnFillCup4Btn);
            base.Bind(_viewModel);
            viewModel.EnterMachine += Enter;
            viewModel.LeaveMachine += Leave;
            viewModel.SpawnNewCup += SpawnNewCup;
            viewModel.FillCup += FillCup;
            viewModel.ForceCupFill += ForceCupFill;
            viewModel.RemoveFilledCup += RemoveFilledCup;
            viewModel.ActivateFillInput += ActivateFillButton;
        }
        private void OnDestroy()
        {
            leaveButton.onClick.RemoveListener(OnLeaveBtn);
            fillCup1MBtn.OnClick.RemoveListener(OnFillCup1Btn);
            fillCup2MBtn.OnClick.RemoveListener(OnFillCup2Btn);
            fillCup3MBtn.OnClick.RemoveListener(OnFillCup3Btn);
            fillCup4MBtn.OnClick.RemoveListener(OnFillCup4Btn);
            if (viewModel != null)
            {
                viewModel.EnterMachine -= Enter;
                viewModel.LeaveMachine -= Leave;
                viewModel.SpawnNewCup -= SpawnNewCup;
                viewModel.FillCup -= FillCup;
                viewModel.ForceCupFill -= ForceCupFill;
                viewModel.RemoveFilledCup -= RemoveFilledCup;
                viewModel.ActivateFillInput -= ActivateFillButton;
            }
        }
        void OnLeaveBtn()
        {
            viewModel?.LeaveInput.OnNext(Unit.Default);
        }
        void OnFillCup1Btn()
        {
            viewModel.FillCupInput.OnNext(0);
        }
        void OnFillCup2Btn()
        {
            viewModel.FillCupInput.OnNext(1);
        }
        void OnFillCup3Btn()
        {
            viewModel.FillCupInput.OnNext(2);
        }
        void OnFillCup4Btn()
        {
            viewModel.FillCupInput.OnNext(3);
        }
        void Enter()
        {
            playerCamController.SetLook(camLookTransform.position, camLookTransform.eulerAngles);
            openVisuals.SetActive(true);
        }
        void Leave()
        {
            playerCamController.ResetLook();
            openVisuals.SetActive(false);
        }
        void SpawnNewCup()
        {
            if(newCupsIgnors > 0)
            {
                newCupsIgnors--;
                return;
            }
            SodaCupObject spawnedCup = Instantiate(cupPrefab, (spawnedCups.Count >= maxCupsPerSection) ? emptyCupsSection2 : emptyCupsSection1);
            spawnedCup.transform.localPosition =
                new Vector3(0, CaclNextCupOffsetY(), 0);
            spawnedCups.Add(spawnedCup);
        }
        void FillCup(int id, Action onFilled)
        {
            if (spawnedCups.Count < 1 || fillCupsPoints.Length <= id) return;
            if (cupsInFill[id] != null) Destroy(cupsInFill[id]);
            cupsInFill[id] = spawnedCups[^1];
            spawnedCups.RemoveAt(spawnedCups.Count - 1);
            DOTween.Sequence()
                .Append(cupsInFill[id].transform.DOJump(fillCupsPoints[id].position, cupJumpToFillForce, 1, cupJumpToFillDuration))
                .Join(cupsInFill[id].transform.DORotate(fillCupsPoints[id].eulerAngles, cupJumpToFillDuration))
                .AppendCallback(() => OnCupReadyToFill(id, onFilled).Forget())
                .Play();
        }
        async UniTaskVoid OnCupReadyToFill(int id, Action onFilled)
        {
            if(sodaFountains.Length > id) sodaFountains[id].SetActive(true);
            cupsInFill[id].FillCup(cupFillDuration);
            await UniTask.WaitForSeconds(cupFillDuration);
            cupsInFill[id].transform.DOLocalMove(cupsInFill[id].transform.localPosition + cupAfterFillLPosAdd, cupAfterFillMoveDur).Play();
            if (sodaFountains.Length > id)  sodaFountains[id].SetActive(false);
            onFilled?.Invoke();
        }
        void ForceCupFill(int id)
        {
            if (spawnedCups.Count < 1 || fillCupsPoints.Length <= id) return;
            if (cupsInFill[id] != null) Destroy(cupsInFill[id]);
            cupsInFill[id] = spawnedCups[^1];
            spawnedCups.RemoveAt(spawnedCups.Count - 1);
            cupsInFill[id].transform.SetPositionAndRotation(fillCupsPoints[id].position, fillCupsPoints[id].rotation);
            cupsInFill[id].FillCup(0);
            cupsInFill[id].transform.localPosition = cupsInFill[id].transform.localPosition + cupAfterFillLPosAdd;
        }
        void RemoveFilledCup(int id)
        {
            if (cupsInFill.Length <= id) return;
            if (cupsInFill[id] != null) 
            {
                orderItemsContainer.AddSodaCup(cupsInFill[id].gameObject);
                cupsInFill[id] = null;
            } 
        }
        void ActivateFillButton(int id, bool activate)
        {
            if (fillBtnsRenderers.Length <= id) return;
            fillBtnsRenderers[id].material = activate ? availableFillBtnMat : notAvailableFillBtnMat;
        }
    }
}
