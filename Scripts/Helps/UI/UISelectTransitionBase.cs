using UnityEngine;

namespace Game.Helps.UI
{
    abstract class UISelectTransitionBase : MonoBehaviour
    {
        public abstract void Select();
        public abstract void Deselect();
    }
}
