using DG.Tweening;
using UnityEngine;

namespace Game.PizzeriaSimulator.Player.CameraController
{
    class SimplePlayerCamController : PlayerCameraControllerBase
    {
        [SerializeField] float sensitivity;
        [SerializeField] float damping;
        [SerializeField] float setLookPosDuration;
        [SerializeField] float setLookRotDuration;
        [SerializeField] float maxXRot;
        [SerializeField] float minXRot;
        [SerializeField] float normalFOV;
        [SerializeField] float maxFOV;
        float currentRotX;
        float targetRotX;
        float rotVelocityX;
        float currentRotY;
        float targetRotY;
        float rotVelocityY;
        float lookFOV;
        float lastScreenRation;
        bool isLocked;
        Vector3 savedLocalPos;
        Tween currentRotTween;
        Tween currentMoveTween;
        Camera mainCam;

        const float normalHandledRatio = 0.7777f;
        const float minScreenRatio = 1f;
        private void Start()
        {
            mainCam = Camera.main;
            ResetRot();
            UpdateLookFOV();
            ResetLook();
        }
        public override void ResetRot()
        {
            transform.localEulerAngles = Vector3.zero;
            currentRotX = 0;
            targetRotX = 0; 
            targetRotY = 0;
            currentRotY = 0;
            savedLocalPos = transform.localPosition;
        }
        public override void Rotate(Vector2 direction)
        {
            if (isLocked) return;
            targetRotY += direction.x * sensitivity;
            targetRotX -= direction.y * sensitivity;
            targetRotX = Mathf.Clamp(targetRotX, minXRot, maxXRot);
            currentRotX = Mathf.SmoothDamp(currentRotX, targetRotX, ref rotVelocityX, damping * Time.deltaTime);
            currentRotY = Mathf.SmoothDamp(currentRotY, targetRotY, ref rotVelocityY, damping * Time.deltaTime);
            player.transform.eulerAngles = new Vector3(player.transform.eulerAngles.x, currentRotY, player.transform.eulerAngles.z);
            transform.localEulerAngles = new Vector3(currentRotX, 0f, 0f);
        }
        public override void SetLook(Vector3 position, Vector3 eulerRotation)
        {
            UpdateLookFOV();
            mainCam.fieldOfView = lookFOV;
            isLocked = true;
            RaiseLockEvent(true);
            if (currentMoveTween.IsActive()) currentMoveTween.Pause();
            if (currentRotTween.IsActive()) currentRotTween.Pause();
            currentMoveTween = transform.DOMove(position, setLookPosDuration).Play();
            currentRotTween = transform.DORotate(eulerRotation, setLookPosDuration).Play();
        }
        void UpdateLookFOV()
        {
            float currentRatio = (float)Screen.width / Screen.height;
            if (currentRatio != lastScreenRation) 
            {
                lastScreenRation = currentRatio;
                lookFOV = Mathf.Lerp(normalFOV, maxFOV, 1 - (currentRatio - minScreenRatio) / normalHandledRatio);
            }
        }
        public override void ResetLook()
        {
            mainCam.fieldOfView = normalFOV;
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
