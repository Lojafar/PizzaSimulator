using Game.PizzeriaSimulator.Player.CameraController;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.PizzeriaSimulator.Computer.Visual
{
    class PizzeriaComputerView : PizzeriaComputerViewBase
    {
        [SerializeField] Transform playerCamLookTransform;
        [SerializeField] Button exitButton;
        [SerializeField] Button marketButton;
        DiContainer diContainer;
        PlayerCameraControllerBase playerCamController;
        [Inject]
        public void Construct(DiContainer _diContainer)
        {
            diContainer = _diContainer;
        }
        public override void Bind(PizzeriaComputerVM _viewModel)
        {
            playerCamController = diContainer.Resolve<PlayerCameraControllerBase>();
            base.Bind(_viewModel);
            exitButton.onClick.AddListener(OnExitBtn);
            marketButton.onClick.AddListener(OnMarketBtn);
            viewModel.EnterComputer += OnEnter;
            viewModel.ExitComputer += OnExit;
        }
        private void OnDestroy()
        {
            exitButton.onClick.RemoveListener(OnExit);
            marketButton.onClick.RemoveListener(OnMarketBtn);
            if (viewModel != null)
            {
                viewModel.EnterComputer -= OnEnter;
                viewModel.ExitComputer -= OnExit;
            }
        }
        void OnExitBtn()
        {
            viewModel.ExitPCInput.OnNext(Unit.Default);
        }
        void OnMarketBtn()
        {
            viewModel.MarketAppInput.OnNext(Unit.Default);
        }
        void OnEnter()
        {
            playerCamController.SetLook(playerCamLookTransform.position, playerCamLookTransform.eulerAngles);
        }
        void OnExit()
        {
            playerCamController.ResetLook();
        }
    }
}
