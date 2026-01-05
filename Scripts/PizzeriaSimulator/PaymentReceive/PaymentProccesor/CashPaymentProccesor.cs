using System;

namespace Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor
{
    using Random = UnityEngine.Random;
    public class CashPaymentProccesor : IPaymentProccesor
    {
        public event Action OnStartProccesing;
        public event Action OnCompleteProccesing;
        public event Action OnFailToComplete;
        public event Action<int, int> OnNewPrice;
        public event Action<int, int> OnNewGive;
        public event Action<int, int> OnNewChange;
        public event Action<int, int, bool> OnNewChangeGived;
        int targetDollars;
        int targetCents;

        int currentChangedDollars;
        int currentChangedCents;

        int targetChangeDollars;
        int targetChangeCents;
        bool confirmed;
        const int maxCentsAmount = 99;
        const int centsInDollar = 100;
        const int rndDollarsDivisFactor = 3;
        Action currentCallback;
        public void ProccesPayment(int dollars, int cents, Action onComplete)
        {
            targetDollars = dollars;
            targetCents = cents;
            currentCallback = onComplete;
            OnNewPrice?.Invoke(dollars, cents);
            CalcRndGive();
            currentChangedDollars = 0;
            currentChangedCents = 0;
            OnGivedChangeChanged(0, 0);
            OnStartProccesing?.Invoke();
        }
        void CalcRndGive()
        {
            int givedDollars = targetDollars + Random.Range(0, targetDollars / rndDollarsDivisFactor + 1);
            int givedCents = targetCents + Random.Range(0, maxCentsAmount + 1);
            if (givedCents > maxCentsAmount) 
            {
                givedDollars++;
                givedCents -= centsInDollar;
            }
            OnNewGive?.Invoke(givedDollars, givedCents);

            targetChangeCents = givedCents - targetCents;
            targetChangeDollars = givedDollars - targetDollars;
            if(targetChangeCents < 0)
            {
                targetChangeDollars--;
                targetChangeCents += centsInDollar;
            }
            OnNewChange?.Invoke(targetChangeDollars, targetChangeCents);
        }
        public void OnGivedChangeChanged(int dollars, int cents)
        {
            currentChangedDollars += dollars;
            currentChangedCents += cents;
            if (currentChangedCents > maxCentsAmount) 
            {
                currentChangedDollars++;
                currentChangedCents -= centsInDollar;
            }
            else if(currentChangedCents < 0)
            {
                currentChangedDollars--;
                currentChangedCents += centsInDollar;
            }
            OnNewChangeGived?.Invoke(currentChangedDollars, currentChangedCents, currentChangedDollars == targetChangeDollars && currentChangedCents == targetChangeCents);
        }
        public void OnConfirmInput()
        {
            if (confirmed) return;
            if (currentChangedDollars != targetChangeDollars || currentChangedCents != targetChangeCents) 
            {
                OnFailToComplete?.Invoke();
                return;
            }
            OnCompleteProccesing?.Invoke();
            confirmed = true;
        }
        public void Confirm()
        {
            if (!confirmed) return;
            confirmed = false;
            currentCallback?.Invoke();
        }
    }
}
