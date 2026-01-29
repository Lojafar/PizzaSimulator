using Game.PizzeriaSimulator.Player.CameraController;
using Game.PizzeriaSimulator.Player.Input;
using Game.PizzeriaSimulator.Player.Movement;
using UnityEngine;

namespace Game.PizzeriaSimulator.Player
{
    public class PizzaPlayer : MonoBehaviour
    {
        [field: SerializeField] public Transform EyesTransform { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public CharacterController PlayerCharController { get; private set; }
        protected IPlayerMovement currentMovement;
        protected IPlayerInput input;
        protected PlayerCameraControllerBase camController;
        public void Construct(IPlayerInput _playerInput, PlayerCameraControllerBase _camController)
        {
            input = _playerInput;
            camController = _camController;
            currentMovement = new DefaultPlayerMovement(this);
        }
        public void Teleport(Transform point)
        {
            PlayerCharController.enabled = false;
            transform.SetPositionAndRotation(point.position, point.rotation);
            PlayerCharController.enabled = true;
            camController.ResetRot();
        }
        private void Update()
        {
            currentMovement?.Move(input.GetMoveDir()); if (camController != null)
            {
                camController.Rotate(input.GetRotationDir());
            }
        }
    }
}
