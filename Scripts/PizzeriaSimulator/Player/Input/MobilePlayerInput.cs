using System;
using UnityEngine;
using UnityEngine.EventSystems;
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
        [SerializeField] GameObject openBoxVisual;
        [SerializeField] GameObject closeBoxVisual;
        [SerializeField] RectTransform rotTouchArea;
        [SerializeField] Button interactButtton;
        [SerializeField] Button throwButton;
        [SerializeField] Button openCloseButton;
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
            throwButton.onClick.AddListener(OnThrowBtn);
            openCloseButton.onClick.AddListener(OnOpenCloseBtn);
            throwButton.gameObject.SetActive(false);
            openCloseButton.gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            interactButtton.onClick.RemoveListener(OnInteractBtn); 
            throwButton.onClick.RemoveListener(OnThrowBtn);
            openCloseButton.onClick.RemoveListener(OnOpenCloseBtn);
        }
        void OnInteractBtn()
        {
            OnInteractInput?.Invoke();
        }
        void OnThrowBtn()
        {
            OnThrowInput?.Invoke();
        }
        void OnOpenCloseBtn()
        {
             OnOpenInput?.Invoke();
        }
        public void SelectInteractInput()
        {
            crosshairImage.color = selectedCrosshairColor;
            interactButtton.interactable = true;
        }
        public void DeselectInteractInput()
        {
            crosshairImage.color = normalCrosshairColor;
            interactButtton.interactable = false;
        }
        public void ShowThrowInput(bool show)
        {
            throwButton.gameObject.SetActive(show);
        }
        public void ShowOpenInput(bool show)
        {
            if(!closeBoxVisual.activeInHierarchy) openCloseButton.gameObject.SetActive(show);
            openBoxVisual.SetActive(show);
        }
        public void ShowCloseInput(bool show)
        {
            if (!openBoxVisual.activeInHierarchy) openCloseButton.gameObject.SetActive(show);
            closeBoxVisual.SetActive(show);
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
                else if(currentRotTouch.phase == TouchPhase.Moved)
                {
                    currentRotDir = -currentRotTouch.deltaPosition / (rotDeltaDivisor * Time.deltaTime);
                }
                else if(currentRotTouch.phase == TouchPhase.Stationary)
                {
                    currentRotDir = Vector2.zero;
                }
            }
            else
            {
                currentRotDir = Vector2.zero;
                currentRotTouchId = -1;
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(touch.fingerId) && RectTransformUtility.RectangleContainsScreenPoint(rotTouchArea, touch.position))
                    {
                        currentRotTouchId = touch.fingerId;
                    }
                }
            }
        }
    }
}
