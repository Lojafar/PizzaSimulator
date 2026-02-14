using System;

namespace Game.PizzeriaSimulator.Orders.Handle.Visual
{
    interface IOrderHandlSubView
    {
        public event Action OnExpandInput;
        public event Action OnCompressInput;
        public void OnExpanded();
        public void OnCompressed();
    }
}
