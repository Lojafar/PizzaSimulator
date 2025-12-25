using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Player.Input
{
    public interface IPlayerInput 
    {
        public event Action OnInteractInput;
        public void SelectInteractInput();
        public void DeselectInteractInput();
        public void Activate(bool active);
        public Vector2 GetMoveDir();
        public Vector2 GetRotationDir();
    }
}
