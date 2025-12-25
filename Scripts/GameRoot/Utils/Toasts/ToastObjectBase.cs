using UnityEngine;

namespace Game.Root.Utils.Toast
{
    abstract class ToastObjectBase : MonoBehaviour
    {
        public abstract void Show(string text);
    }
}
