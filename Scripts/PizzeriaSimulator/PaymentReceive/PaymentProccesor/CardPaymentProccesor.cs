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
        bool confirmed;
        Action currentCallback;
        public void ProccesPayment(int dollars, int cents, Action onComplete)
        {
            targetDollars = dollars;
            targetCents = cents;
            currentCallback = onComplete;
            OnNewPrice?.Invoke(dollars, cents);
            OnStartProccesing?.Invoke();
        }
        public void OnConfirmInput(int dollars, int cents)
        {
            if (confirmed) return;
            if (dollars == targetDollars && cents == targetCents)
            {
                OnCompleteProccesing?.Invoke();
                confirmed = true;
            }
            else
            {
                OnFailToComplete?.Invoke();
            }
        }
        public void OnCompleteShowEnded()
        {
            if (!confirmed) return;
            confirmed = false;
            currentCallback?.Invoke();
        }
    }
}
