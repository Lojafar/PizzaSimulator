using Game.PizzeriaSimulator.DayCycle.Manager;
using Game.PizzeriaSimulator.Player.CameraController;
using Game.PizzeriaSimulator.Player.Input;
using Game.Root.ServicesInterfaces;

namespace Game.PizzeriaSimulator.Player.Handler
{
    class PizzaPlayerHandler : IInittable, ISceneDisposable
    {
        public int InitPriority => 10;
        readonly IPlayerInput playerInput;
        readonly PlayerCameraControllerBase cameraController;
        readonly PizzaPlayer player;
        readonly DayCycleManager dayCycleManager;
        readonly PizzeriaSceneReferences sceneReferences;
        bool inputDeactivated;
        public PizzaPlayerHandler(IPlayerInput _playerInput, PlayerCameraControllerBase _cameraController, PizzaPlayer _player, 
            DayCycleManager _dayCycleManager, PizzeriaSceneReferences _sceneReferences) 
        {
            playerInput = _playerInput;
            cameraController = _cameraController;
            player = _player;
            dayCycleManager = _dayCycleManager;
            sceneReferences = _sceneReferences;
        }
        public void Init()
        {
            cameraController.OnCameraLocked += HandleCameraLock;
            dayCycleManager.OnDayStarted += HandleDayStart;
        }
        public void Dispose()
        {
            cameraController.OnCameraLocked -= HandleCameraLock;
            dayCycleManager.OnDayStarted -= HandleDayStart;
        }
        void HandleCameraLock(bool locked)
        {
            if (locked == inputDeactivated) return;
            playerInput.Activate(!locked);
            inputDeactivated = locked;
        }
        void HandleDayStart()
        {
            player.Teleport(sceneReferences.PlayerSpawnPoint);
        }
    }
}
