using UnityEngine;

namespace Game.PizzeriaSimulator.Delivery.Visual
{
    abstract class PizzeriaDeliveryViewBase : MonoBehaviour
    {
        protected PizzeriaDeliveryVM viewModel;
        public virtual void Bind(PizzeriaDeliveryVM _viewModel)
        {
            viewModel = _viewModel;
        }
    }
}
