using DG.Tweening;
using UnityEngine;

namespace Game.PizzeriaSimulator.Player.CameraController
{
    class SimplePlayerCamController : PlayerCameraControllerBase
    {
        [SerializeField] float sensitivity;
        [SerializeField] float setLookPosDuration;
        [SerializeField] float setLookRotDuration;
        float currentRotX;
        float currentRotY;
        bool isLocked;
        Vector3 savedLocalPos;
        Tween currentRotTween;
        Tween currentMoveTween;
        private void Start()
        {
            currentRotX = transform.eulerAngles.x;
            savedLocalPos = transform.localPosition;
            ResetLook();
        }
        public override void Rotate(Vector2 direction)
        {
            if (isLocked) return;
            currentRotY += direction.x * sensitivity;
            currentRotX -= direction.y * sensitivity;
            Vector3 playerEulerAngles = player.transform.eulerAngles;
            player.transform.eulerAngles = new Vector3(playerEulerAngles.x, currentRotY, playerEulerAngles.z);
            transform.localEulerAngles = new Vector3(currentRotX, 0f, 0f);
        }
        public override void SetLook(Vector3 position, Vector3 eulerRotation)
        {
            isLocked = true;
            RaiseLockEvent(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (currentMoveTween.IsActive()) currentMoveTween.Pause();
            if (currentRotTween.IsActive()) currentRotTween.Pause();
            currentMoveTween = transform.DOMove(position, setLookPosDuration).Play();
            currentRotTween = transform.DORotate(eulerRotation, setLookPosDuration).Play();
        }
        public override void ResetLook()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (currentMoveTween.IsActive()) currentMoveTween.Pause();
            if (currentRotTween.IsActive()) currentRotTween.Pause();
            currentMoveTween = transform.DOLocalMove(savedLocalPos, setLookPosDuration).OnComplete(OnReseted).Play();
            currentRotTween = transform.DOLocalRotate(new Vector3(currentRotX, 0f, 0f), setLookPosDuration).Play();
        }
        void OnReseted()
        {
            isLocked = false;
            RaiseLockEvent(false);
        }
    }
}
