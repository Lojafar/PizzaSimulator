using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Player.CameraController
{
    public abstract class PlayerCameraControllerBase : MonoBehaviour
    {
        public event Action<bool> OnCameraLocked;
        protected PizzaPlayer player;
        public virtual void Construct(PizzaPlayer _player)
        {
            player = _player;
        }
        protected void RaiseLockEvent(bool locked) 
        { 
            OnCameraLocked?.Invoke(locked);
        }
        public abstract void Rotate(Vector2 direction);
        public abstract void SetLook(Vector3 position, Vector3 eulerRotation);
        public abstract void ResetLook();
    }
}
