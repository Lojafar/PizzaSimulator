using UnityEngine;

namespace Game.PizzeriaSimulator.UI.EndDayStatsPanel.Visual
{
    abstract class EndDayStatsViewBase : MonoBehaviour
    {
        protected EndDayStatsVM viewModel;
        public virtual void Bind(EndDayStatsVM _viewModel)
        {
            viewModel = _viewModel;
        }
    }
}
