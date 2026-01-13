using UnityEngine;

namespace Game.PizzeriaSimulator.Computer.Visual
{
    public abstract class PizzeriaComputerViewBase : MonoBehaviour
    {
        protected PizzeriaComputerVM viewModel;
        public virtual void Bind(PizzeriaComputerVM _viewModel)
        {
            viewModel = _viewModel;
        }
    }
}
