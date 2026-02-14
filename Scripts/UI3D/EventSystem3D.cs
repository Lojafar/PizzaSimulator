using Game.UI3D.Elements;
using UnityEngine;

namespace Game.UI3D
{
    public sealed class EventSystem3D : MonoBehaviour
    {
        Camera mainCam;
        int ui3DRayMask;
        const float maxRayDistance = 30f;
        private void Awake()
        {
            ui3DRayMask = LayerMask.GetMask("UI");
        }
        void Update()
        {
            if(Input.GetMouseButtonUp(0) && Cursor.visible)
            {
                if (mainCam == null)
                {
                    mainCam = Camera.main;
                    if (mainCam == null) return;
                }
                CheckUIRay();
            }
        }
        void CheckUIRay()
        {
            if(Physics.Raycast(mainCam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, maxRayDistance, ui3DRayMask)
                && hit.collider.gameObject.TryGetComponent(out IClickableElement clickableElement))
            {
                clickableElement.Click();
            }
        }
    }
}
