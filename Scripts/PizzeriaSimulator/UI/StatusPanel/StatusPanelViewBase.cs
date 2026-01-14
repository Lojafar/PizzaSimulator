using UnityEngine;

namespace Game.PizzeriaSimulator.UI.StatusPanel
{
    abstract class StatusPanelViewBase : MonoBehaviour
    {
        protected StatusPanelVM viewModel;
        public virtual void Bind(StatusPanelVM _viewModel)
        {
            viewModel = _viewModel;
        }
    }
}
