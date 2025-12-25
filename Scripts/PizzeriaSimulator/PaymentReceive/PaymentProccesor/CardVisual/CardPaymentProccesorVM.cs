using System;
using Game.Root.ServicesInterfaces;
using R3;

namespace Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor.Visual
{
    public class CardPaymentProccesorVM : ISceneDisposable
    {
        public event Action OnStartProcces;
        public event Action OnCompleteProcces;
        public event Action<string> UpdatePriceText;
        public event Action<string> UpdateEnterText;

        public Subject<Unit> ConfirmInput;
        public Subject<Unit> BackInput;
        public Subject<Unit> DotInput;
        public Subject<int> DigitInput;

        int enteredDollars;
        int enteredCents;
        int enteredCentsDigits;
        int enteredDollarsDigits;
        bool isDotEntered;

        readonly CardPaymentProccesor cardPaymentProccesor;
        const string moneyValuePattern = "{0}.{1:D2}";
        const int maxEnteredDigits = 10;
        public CardPaymentProccesorVM(CardPaymentProccesor _cardPaymentProccesor)
        {
            cardPaymentProccesor = _cardPaymentProccesor;
            ConfirmInput = new Subject<Unit>();
            BackInput = new Subject<Unit>();
            DotInput = new Subject<Unit>();
            DigitInput = new Subject<int>();
        }
        public void Init()
        {
            BackInput.ThrottleFirst(TimeSpan.FromSeconds(0.05f)).Subscribe(_ => HandleBackInput());
            ConfirmInput.ThrottleFirst(TimeSpan.FromSeconds(0.1f)).Subscribe(_ => HandleConfirmInput());
            DotInput.ThrottleFirst(TimeSpan.FromSeconds(0.1f)).Subscribe(_ => HandleDotInput());
            DigitInput.ThrottleFirst(TimeSpan.FromSeconds(0.1f)).Subscribe(HandleDigitInput);
            cardPaymentProccesor.OnStartProccesing += OnStartProccesing;
            cardPaymentProccesor.OnCompleteProccesing += OnCompleteProccesing;
            cardPaymentProccesor.OnNewPrice += HandleNewPrice;
        }
        public void Dispose()
        {
            BackInput.Dispose();
            ConfirmInput.Dispose();
            DotInput.Dispose();
            DigitInput.Dispose();
            cardPaymentProccesor.OnStartProccesing -= OnStartProccesing;
            cardPaymentProccesor.OnCompleteProccesing -= OnCompleteProccesing;
            cardPaymentProccesor.OnNewPrice -= HandleNewPrice;
        }
        void HandleBackInput()
        {
            if (isDotEntered)
            {
                if(enteredCentsDigits > 0)
                {
                    enteredCents /= 10;
                    enteredCentsDigits--;
                }
                else
                {
                    isDotEntered = false;
                }
            }
            else
            {
                enteredDollarsDigits--;
                if (enteredDollarsDigits < 0) enteredDollars = 0;
                enteredDollars /= 10;
            }

            if (isDotEntered)
            {
                if(enteredCentsDigits > 0) UpdateEnterText?.Invoke(string.Format(enteredCentsDigits == 1 ? "{0}.{1:D1}" : "{0}.{1:D2}", enteredDollars, enteredCents));
                else UpdateEnterText?.Invoke($"{enteredDollars}.");
            }
            else UpdateEnterText?.Invoke($"{enteredDollars}");
        }
        void HandleConfirmInput()
        {
            cardPaymentProccesor.OnConfirmInput(enteredDollars, enteredCentsDigits < 2 ? enteredCents * 10 : enteredCents);
        }
        void HandleDotInput()
        {
            if (IsEnterFull())
            {
                // more that ten digits
                return;
            }
            isDotEntered = true;
            UpdateEnterText?.Invoke($"{enteredDollars}.");
        }
        void HandleDigitInput(int digit)
        {
            if (digit < 0 || digit > 9) return;
            if(IsEnterFull())
            {
                // more that ten digits
                return;
            }
            if (isDotEntered)
            {
                if (enteredCentsDigits > 1)
                {
                    // more than two cents
                    return;
                }
                enteredCentsDigits++;
                enteredCents = enteredCents * 10 + digit;
                UpdateEnterText?.Invoke(string.Format(enteredCentsDigits == 1 ? "{0}.{1:D1}" : "{0}.{1:D2}", enteredDollars, enteredCents));
            }
            else
            {
                enteredDollarsDigits++;
                enteredDollars = enteredDollars * 10 + digit;
                UpdateEnterText?.Invoke($"{enteredDollars}");
            }

        }
        bool IsEnterFull()
        {
            return (enteredDollarsDigits + enteredCentsDigits + (isDotEntered ? 1 : 0)) > maxEnteredDigits;
        }
        void OnStartProccesing()
        {
            ClearEnter();
            OnStartProcces?.Invoke();
        }
        void ClearEnter()
        {
            enteredDollars = 0;
            enteredCents = 0;
            enteredCentsDigits = 0;
            enteredDollarsDigits = 0;
            isDotEntered = false;
            HandleDigitInput(0);
        }
        void OnCompleteProccesing()
        {
            OnCompleteProcces?.Invoke();
        }
        void HandleNewPrice(int dollars, int cents)
        {
            UpdatePriceText?.Invoke(string.Format(moneyValuePattern, dollars, cents));
        }
    }
}
