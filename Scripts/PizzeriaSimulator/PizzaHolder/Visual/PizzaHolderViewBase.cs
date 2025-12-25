using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaHold.Visual
{
    public abstract class PizzaHolderViewBase : MonoBehaviour
    {
        protected PizzaHolderVM viewModel;
        public virtual void Bind(PizzaHolderVM _viewModel)
        {
            viewModel = _viewModel;
        }
    }
}
