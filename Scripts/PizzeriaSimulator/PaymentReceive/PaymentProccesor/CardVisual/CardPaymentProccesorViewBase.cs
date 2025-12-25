using UnityEngine;

namespace Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor.Visual
{
    public abstract class CardPaymentProccesorViewBase : MonoBehaviour
    {
        protected CardPaymentProccesorVM viewModel;
        public virtual void Bind(CardPaymentProccesorVM _viewModel)
        {
            viewModel = _viewModel;
        }
    }
}
