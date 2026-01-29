using UnityEngine;

namespace Assets.Game.Scripts.PizzeriaSimulator.UI.NewLevelPopUp.Visual
{
    abstract class NewLevelPopUpViewBase : MonoBehaviour
    {
        protected NewLevelPopUpVM viewModel;
        public virtual void Bind(NewLevelPopUpVM _viewModel)
        {
            viewModel = _viewModel;
        }
    }
}
