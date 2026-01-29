using UnityEngine;

namespace Game.PizzeriaSimulator.Tutorial.Visual
{
    abstract class TutorialViewBase : MonoBehaviour
    {
        protected TutorialVM viewModel;
        public virtual void Bind(TutorialVM _viewModel)
        {
            viewModel = _viewModel;
        }
    }
}