using UnityEngine;

namespace Game.PizzeriaSimulator.OrdersHandle.Visual
{
    public abstract class PizzeriaOrderHandlViewBase : MonoBehaviour
    {
        protected PizzeriaOrderHandlVM viewModel;
        public virtual void Bind(PizzeriaOrderHandlVM _viewModel)
        {
            viewModel = _viewModel;
        }
    }
}
