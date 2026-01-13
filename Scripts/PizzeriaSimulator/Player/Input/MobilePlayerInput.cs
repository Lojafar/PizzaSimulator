using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PizzeriaSimulator.Player.Input
{
    using Input = UnityEngine.Input;
    class MobilePlayerInput : MonoBehaviour, IPlayerInput
    {
        public event Action OnInteractInput;
        public event Action OnThrowInput;
        public event Action OnOpenInput;
        [SerializeField] Image crosshairImage;
        [SerializeField] RectTransform rotTouchArea;
        [SerializeField] Button interactButtton;
        [SerializeField] Joystick movementJoystick;
        [SerializeField] Color normalCrosshairColor = Color.white;
        [SerializeField] Color selectedCrosshairColor = Color.white;
        [SerializeField] float rotDeltaDivisor = 1;
        Vector2 currentMoveDir;
        Vector2 currentRotDir;

        int currentRotTouchId = -1;
        private void Awake()
        {
            DeselectInteractInput();
            interactButtton.onClick.AddListener(OnInteractBtn);
        }
        private void OnDestroy()
        {
            interactButtton.onClick.RemoveListener(OnInteractBtn);
        }
        void OnInteractBtn()
        {
            OnInteractInput?.Invoke();
        }
        public void SelectInteractInput()
        {
            crosshairImage.color = selectedCrosshairColor;
            interactButtton.gameObject.SetActive(true);
        }
        public void DeselectInteractInput()
        {
            crosshairImage.color = normalCrosshairColor;
            interactButtton.gameObject.SetActive(false);
        }
        public void Activate(bool active)
        {
            currentRotTouchId = -1;
            currentRotDir = Vector2.zero;
            currentMoveDir = Vector2.zero;
            gameObject.SetActive(active);
        }
        public Vector2 GetMoveDir()
        {
            return currentMoveDir; 
        }
        public Vector2 GetRotationDir()
        {
            return currentRotDir;
        }
        private void Update()
        {
            currentMoveDir = movementJoystick.Direction;
            UpdateTouchRotate();
        }
        void UpdateTouchRotate()
        {
            if (currentRotTouchId > -1 && Input.touchCount > currentRotTouchId)
            {
                Touch currentRotTouch = Input.GetTouch(currentRotTouchId);
                if (currentRotTouch.phase == TouchPhase.Ended || currentRotTouch.phase == TouchPhase.Canceled)
                {
                    currentRotDir = Vector2.zero;
                    currentRotTouchId = -1;
                }
                else
                {
                    currentRotDir = currentRotTouch.deltaPosition / rotDeltaDivisor;
                }
            }
            else
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began && RectTransformUtility.RectangleContainsScreenPoint(rotTouchArea, touch.position))
                    {
                        currentRotTouchId = touch.fingerId;
                    }
                }
            }
        }
    }
}
