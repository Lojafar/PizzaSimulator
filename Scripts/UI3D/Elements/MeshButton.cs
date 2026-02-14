using UnityEngine;
using UnityEngine.Events;

namespace Game.UI3D.Elements
{
    [RequireComponent(typeof(Collider))]
    public class MeshButton : MonoBehaviour, IClickableElement
    {
        public UnityEvent OnClick;
        public void Click()
        {
            OnClick?.Invoke();
        }
        private void OnDestroy()
        {
            OnClick.RemoveAllListeners();
            OnClick = null;
        }
    }
}
