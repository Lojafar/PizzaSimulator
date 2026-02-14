using UnityEngine;
namespace Game.PizzeriaSimulator.SodaMachine.Visual
{
    abstract class PizzeriaSodaMachineViewBase : MonoBehaviour
    {
        protected PizzeriaSodaMachineVM viewModel;
        public abstract (Vector3, Vector3) GetNextCupPoint();
        public abstract void PrepareForExternalCup();
        public abstract void AddCupExternal(SodaCupObject cupObject);
        public virtual void Bind(PizzeriaSodaMachineVM _viewModel)
        {
            viewModel = _viewModel;
        }
    }
}
