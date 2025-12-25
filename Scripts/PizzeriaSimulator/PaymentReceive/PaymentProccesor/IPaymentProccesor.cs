using System;

namespace Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor
{
    public interface IPaymentProccesor
    {
        public void ProccesPayment(int dollars, int cents, Action onComplete);
    }
}
