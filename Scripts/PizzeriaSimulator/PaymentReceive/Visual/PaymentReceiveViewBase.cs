using UnityEngine;
namespace Game.PizzeriaSimulator.PaymentReceive.Visual
{
    public abstract class PaymentReceiveViewBase : MonoBehaviour
    {
        protected PaymentReceiverVM viewModel;
        public virtual void Bind(PaymentReceiverVM _viewModel)
        {
            viewModel = _viewModel;
        }
    }
}
