using UnityEngine;
namespace Game.PizzeriaSimulator.PizzaCreation.Visual
{
    public abstract class PizzaCreatorViewBase : MonoBehaviour
    {
        protected PizzaCreatorVM viewModel;
        public virtual void Bind(PizzaCreatorVM _viewModel)
        {
            viewModel = _viewModel;
        }
    }
}
