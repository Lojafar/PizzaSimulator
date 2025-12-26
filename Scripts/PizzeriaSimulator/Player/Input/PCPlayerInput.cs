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
        Vector2 movementDir;
        Vector2 rotDir;
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
            Cursor.lockState = active ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !active;
            gameObject.SetActive(active);
            if (!active)
            {
                movementDir = Vector2.zero;
                rotDir = Vector2.zero;
            }
        }
        public Vector2 GetMoveDir()
        {
            return movementDir; 
        }
        public Vector2 GetRotationDir()
        {
            return rotDir; 
        }
        private void Update()
        {
            movementDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            rotDir = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            if (Input.GetKeyDown(KeyCode.E))
            {
                OnInteractInput?.Invoke();
            }
        }
    }
}
