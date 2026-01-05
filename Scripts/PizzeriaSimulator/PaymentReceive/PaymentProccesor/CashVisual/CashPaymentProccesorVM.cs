using System;
using System.Collections.Generic;
using Game.Root.ServicesInterfaces;
using R3;
using UnityEngine;

namespace Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor.Visual
{
    public class CashPaymentProccesorVM : ISceneDisposable
    {
        public event Action OnStartProcces;
        public event Action<Action> OnCompleteProcces;
        public event Action OnConfirmedActiveCash;
        public event Action OnCancelledLastCash;
        public event Action OnClearedAllCash;
        public event Action<string> UpdatePriceText;
        public event Action<string> UpdateGiveText;
        public event Action<string> UpdateChangeText;
        public event Action<string> UpdateGivedChangeText;
        public event Action<string> ShowErrorMessage;
        public event Action<bool> SetGivedChangeIsTarget;

        public Subject<Unit> ConfirmInput;
        public Subject<Unit> BackInput;
        public Subject<Unit> ClearInput;
        public Subject<int> ChangeInput;

        readonly Stack<int> givedChangeStack;
        readonly CashPaymentProccesor cashPaymentProccesor;
        const string moneyValuePattern = "{0}.{1:D2}";
        const int centsInDollar = 100;
        public CashPaymentProccesorVM(CashPaymentProccesor _cashPaymentProccesor)
        {
            cashPaymentProccesor = _cashPaymentProccesor;
            ConfirmInput = new Subject<Unit>();
            BackInput = new Subject<Unit>();
            ClearInput = new Subject<Unit>();
            ChangeInput = new Subject<int>();
            givedChangeStack  = new Stack<int>();
        }
        public void Init()
        {
            ChangeInput.ThrottleFirst(TimeSpan.FromSeconds(0.05f)).Subscribe(HandleChangeInput);
            BackInput.ThrottleFirst(TimeSpan.FromSeconds(0.05f)).Subscribe(_ => HandleBackInput());
            ClearInput.ThrottleFirst(TimeSpan.FromSeconds(0.05f)).Subscribe(_ => HandleClearInput());
            ConfirmInput.ThrottleFirst(TimeSpan.FromSeconds(0.1f)).Subscribe(_ => cashPaymentProccesor.OnConfirmInput());
            cashPaymentProccesor.OnStartProccesing += OnStartProccesing;
            cashPaymentProccesor.OnCompleteProccesing += OnCompleteProccesing;
            cashPaymentProccesor.OnNewPrice += HandleNewPrice;
            cashPaymentProccesor.OnNewGive += HandleNewGiving;
            cashPaymentProccesor.OnNewChange += HandleNewChange;
            cashPaymentProccesor.OnNewChangeGived += HandleGivedChange;
            cashPaymentProccesor.OnFailToComplete += HandleFailComplete;
        }
        public void Dispose()
        {
            ChangeInput.Dispose();
            BackInput.Dispose();
            ClearInput.Dispose();
            ConfirmInput.Dispose();

            cashPaymentProccesor.OnStartProccesing -= OnStartProccesing;
            cashPaymentProccesor.OnCompleteProccesing -= OnCompleteProccesing;
            cashPaymentProccesor.OnNewPrice -= HandleNewPrice;
            cashPaymentProccesor.OnNewGive -= HandleNewGiving;
            cashPaymentProccesor.OnNewChange -= HandleNewChange;
            cashPaymentProccesor.OnNewChangeGived -= HandleGivedChange;
            cashPaymentProccesor.OnFailToComplete -= HandleFailComplete;
        }
        void HandleChangeInput(int amount)
        {
            if (amount < 0) return;
            givedChangeStack.Push(amount);
            int dollars = Mathf.FloorToInt((float)amount / centsInDollar);
            cashPaymentProccesor.OnGivedChangeChanged(dollars, amount - dollars * centsInDollar);
            OnConfirmedActiveCash?.Invoke();
        }
        void HandleBackInput()
        {
            if (givedChangeStack.Count < 1) return;
            int lastGived = givedChangeStack.Pop();
            int dollars = Mathf.FloorToInt((float)lastGived / centsInDollar);
            cashPaymentProccesor.OnGivedChangeChanged(-dollars, -lastGived + dollars * centsInDollar);
            OnCancelledLastCash?.Invoke();
        }
        void HandleClearInput()
        {
            foreach(int gived in givedChangeStack)
            {
                int dollars = Mathf.FloorToInt((float)gived / centsInDollar);
                cashPaymentProccesor.OnGivedChangeChanged(-dollars, -gived + dollars * centsInDollar);
            }
            OnClearedAllCash?.Invoke();
            givedChangeStack.Clear();
        }
        void OnStartProccesing()
        {
            givedChangeStack.Clear();
            OnStartProcces?.Invoke();
        }  
        void OnCompleteProccesing()
        {
            OnCompleteProcces?.Invoke(cashPaymentProccesor.Confirm);
        }
        void HandleNewPrice(int dollars, int cents)
        {
            UpdatePriceText?.Invoke(string.Format(moneyValuePattern, dollars, cents));
        }  
        void HandleNewGiving(int dollars, int cents)
        {
            UpdateGiveText?.Invoke(string.Format(moneyValuePattern, dollars, cents));
        }  
        void HandleNewChange(int dollars, int cents)
        {
            UpdateChangeText?.Invoke(string.Format(moneyValuePattern, dollars, cents));
        }
        void HandleGivedChange(int dollars, int cents, bool isTargetChange)
        {
            UpdateGivedChangeText?.Invoke(string.Format(moneyValuePattern, dollars, cents));
            SetGivedChangeIsTarget?.Invoke(isTargetChange);
        }
        void HandleFailComplete()
        {
            ShowErrorMessage?.Invoke("Wrong Change!");
        }
    }
}
