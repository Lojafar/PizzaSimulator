using UnityEngine;

namespace Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor.Visual
{
    public abstract class CashPaymentProccesorViewBase : MonoBehaviour
    {
        protected CashPaymentProccesorVM viewModel;
        public virtual void Bind(CashPaymentProccesorVM _viewModel)
        {
            viewModel = _viewModel;
        }
    }
}
