using Game.PizzeriaSimulator.Player.CameraController;
using Game.PizzeriaSimulator.Player.Input;
using Game.Root.ServicesInterfaces;

namespace Game.PizzeriaSimulator.Player.Handler
{
    class PizzaPlayerHandler : ISceneDisposable
    {
        readonly IPlayerInput playerInput;
        readonly PlayerCameraControllerBase cameraController;
        public PizzaPlayerHandler(IPlayerInput _playerInput, PlayerCameraControllerBase _cameraController) 
        {
            playerInput = _playerInput;
            cameraController = _cameraController;
        }
        public void Init()
        {
            cameraController.OnCameraLocked += HandleCameraLock;
        }
        public void Dispose()
        {
            cameraController.OnCameraLocked -= HandleCameraLock;
        }
        void HandleCameraLock(bool locked)
        {
            playerInput.Activate(!locked);
        }
    }
}
