using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Player.Input
{
    public interface IPlayerInput 
    {
        public event Action OnInteractInput;
        public event Action OnThrowInput;
        public event Action OnOpenInput;
        public void SelectInteractInput();
        public void DeselectInteractInput();
        public void ShowThrowInput(bool show);
        public void ShowOpenInput(bool show);
        public void ShowCloseInput(bool show);
        public void Activate(bool active);
        public Vector2 GetMoveDir();
        public Vector2 GetRotationDir();
    }
}
