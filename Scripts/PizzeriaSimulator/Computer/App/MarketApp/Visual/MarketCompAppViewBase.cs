using UnityEngine;

namespace Game.PizzeriaSimulator.Computer.App.Market.Visual
{
    public abstract class MarketCompAppViewBase : MonoBehaviour
    {
        protected MarketCompAppVM viewModel;
        public virtual void Bind(MarketCompAppVM _viewModel)
        {
            viewModel = _viewModel;
        }
    }
}
