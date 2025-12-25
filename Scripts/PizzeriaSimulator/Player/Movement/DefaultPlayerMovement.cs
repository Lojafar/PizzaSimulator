using UnityEngine;
namespace Game.PizzeriaSimulator.Player.Movement
{
    class DefaultPlayerMovement : IPlayerMovement
    {
        readonly PizzaPlayer player;
        readonly CharacterController characterController;
        public DefaultPlayerMovement(PizzaPlayer _player)
        {
            player = _player;
            characterController = player.PlayerCharController;
        }
        public void Move(Vector2 direction)
        {
            if(direction.sqrMagnitude > 1)
            {
                direction.Normalize();
            }
            Vector3 moveVector = player.transform.TransformDirection( new Vector3(direction.x * player.Speed, 0, direction.y * player.Speed ));
            characterController.SimpleMove(moveVector * Time.deltaTime);
        }
    }
}
