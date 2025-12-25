using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PizzeriaSimulator.Player.Input
{
    using Input = UnityEngine.Input;
    class PCPlayerInput : MonoBehaviour, IPlayerInput
    {
        [SerializeField] Image crosshairImage;
        [SerializeField] Color normalCrosshairColor = Color.white;
        [SerializeField] Color selectedCrosshairColor = Color.white;
        public event Action OnInteractInput;
        Vector2 currentMoveDir;
        Vector2 currentRotDir;
        private void Awake()
        {
            DeselectInteractInput();
        }
        public void SelectInteractInput()
        {
            crosshairImage.color = selectedCrosshairColor;
        }
        public void DeselectInteractInput()
        {
            crosshairImage.color = normalCrosshairColor;
        }
        public void Activate(bool active)
        {
            currentMoveDir = Vector2.zero;
            currentRotDir = Vector2.zero;
            gameObject.SetActive(active);
        }
        public Vector2 GetMoveDir()
        {
            return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        public Vector2 GetRotationDir()
        {
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
        private void Update()
        {
            currentMoveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            currentRotDir = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            if (Input.GetKeyDown(KeyCode.E))
            {
                OnInteractInput?.Invoke();
            }
        }
    }
}
