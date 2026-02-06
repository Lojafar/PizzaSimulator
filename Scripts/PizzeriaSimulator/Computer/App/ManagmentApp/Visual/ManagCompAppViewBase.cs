using UnityEngine;

namespace Game.PizzeriaSimulator.Computer.App.ManagmentApp.Visual
{
    public abstract class ManagCompAppViewBase : MonoBehaviour
    {
        protected ManagmentCompAppVM viewModel;
        public virtual void Bind(ManagmentCompAppVM _viewModel)
        {
            viewModel = _viewModel;
        }
    }
}
