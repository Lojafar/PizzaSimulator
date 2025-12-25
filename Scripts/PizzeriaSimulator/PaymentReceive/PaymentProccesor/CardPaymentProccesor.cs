using System;

namespace Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor
{
     public class CardPaymentProccesor : IPaymentProccesor
    {
        public event Action OnStartProccesing;
        public event Action OnCompleteProccesing;
        public event Action OnFailToComplete;
        public event Action<int, int> OnNewPrice;
        int targetDollars;
        int targetCents;
        Action currentCallback;
        public void ProccesPayment(int dollars, int cents, Action onComplete)
        {
            targetDollars = dollars;
            targetCents = cents;
            OnNewPrice?.Invoke(dollars, cents);
            currentCallback = onComplete;
            OnStartProccesing?.Invoke();
        }
        public void OnConfirmInput(int dollars, int cents)
        {
            if (dollars == targetDollars && cents == targetCents)
            {
                OnCompleteProccesing?.Invoke();
                currentCallback?.Invoke();
            }
            else
            {
                OnFailToComplete?.Invoke();
            }
        }
    }
}
