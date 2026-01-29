using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PizzeriaSimulator.Player.Input
{
    using Input = UnityEngine.Input;
    class PCPlayerInput : MonoBehaviour, IPlayerInput
    {
        public event Action OnInteractInput;
        public event Action OnThrowInput;
        public event Action OnOpenInput;
        [SerializeField] GameObject useHint;
        [SerializeField] GameObject throwHint;
        [SerializeField] GameObject openHint;
        [SerializeField] GameObject closeHint;
        [SerializeField] Image crosshairImage;
        [SerializeField] Color normalCrosshairColor = Color.white;
        [SerializeField] Color selectedCrosshairColor = Color.white;
        int activations;
        Vector2 movementDir;
        Vector2 rotDir; 
        private void Awake()
        {
            DeselectInteractInput();
            throwHint.SetActive(false);
            openHint.SetActive(false);
            closeHint.SetActive(false);
        }
        public void SelectInteractInput()
        {
            crosshairImage.color = selectedCrosshairColor;
            useHint.SetActive(true);
        }
        public void DeselectInteractInput()
        {
            crosshairImage.color = normalCrosshairColor;
            useHint.SetActive(false);
        }
        public void ShowThrowInput(bool show)
        {
            throwHint.SetActive(show);
        }
        public void ShowOpenInput(bool show)
        {
            openHint.SetActive(show);
        }
        public void ShowCloseInput(bool show)
        {
            closeHint.SetActive(show);
        }
        public void Activate(bool active)
        {
            int newActivations = activations + (active ? 1 : -1);
            if (activations != 0 && newActivations != 0)
            {
                activations = newActivations;
                return;
            }
            activations = newActivations;
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
            else if (Input.GetKeyDown(KeyCode.Q)) 
            {
                OnThrowInput?.Invoke();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                OnOpenInput?.Invoke();
            }
        }
    }
}
