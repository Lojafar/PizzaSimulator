using UnityEngine;

namespace Game.PizzeriaSimulator.Orders.Handle.Visual
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