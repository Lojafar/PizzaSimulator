using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Orders.Handle.Visual
{
    abstract class OrderHandlSubViewBase : MonoBehaviour
    {
        public abstract event Action OnExpandInput;
        public abstract event Action OnCompressInput;
        public virtual void OnExpanded() { }
        public virtual void OnCompressed() { }
        public virtual void UpdateCompress(bool compressed) { }
    }
}
