using Game.PizzeriaSimulator.Player.CameraController;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
namespace Game.PizzeriaSimulator.PaymentReceive.Visual
{
    class PaymentReceiverView : PaymentReceiveViewBase
    {
        [SerializeField] Transform playerCamLookTransform;
        [SerializeField] GameObject receiveWaitPanel;
        [SerializeField] Button leaveButton;
        DiContainer diContainer;
        PlayerCameraControllerBase playerCamController;

        [Inject]
        public void Construct(DiContainer _diContainer)
        {
            diContainer = _diContainer;
        }
        public override void Bind(PaymentReceiverVM _viewModel)
        {
            playerCamController = diContainer.Resolve<PlayerCameraControllerBase>();
            base.Bind(_viewModel);
            leaveButton.onClick.AddListener(OnLeaveBtn);
            viewModel.OnEnterReceive += OnEnterToReceive;
            viewModel.OnLeaveReceive += OnExitFromReceive;
            viewModel.OnStartReceiving += OnStartReceiving;
            viewModel.OnReceived += OnReceived;
        }
        private void OnDestroy()
        {
            leaveButton.onClick.RemoveListener(OnLeaveBtn);
            if (viewModel != null)
            {
                viewModel.OnEnterReceive -= OnEnterToReceive;
                viewModel.OnLeaveReceive -= OnExitFromReceive;
                viewModel.OnStartReceiving -= OnStartReceiving;
                viewModel.OnReceived -= OnReceived;
            }
        }
        void OnLeaveBtn()
        {
            viewModel.LeaveInput.OnNext(Unit.Default);
        }
        void OnEnterToReceive()
        {
            playerCamController.SetLook(playerCamLookTransform.position, playerCamLookTransform.eulerAngles);
        }
        void OnExitFromReceive()
        {
            playerCamController.ResetLook();
        }

        void OnStartReceiving()
        {
            receiveWaitPanel.SetActive(false);
            playerCamController.ResetLook();
        }
        void OnReceived()
        {
            receiveWaitPanel.SetActive(true);
            playerCamController.SetLook(playerCamLookTransform.position, playerCamLookTransform.eulerAngles);
        }
    }
}
