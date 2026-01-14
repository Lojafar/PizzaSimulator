using Game.PizzeriaSimulator.Currency;
using Game.PizzeriaSimulator.Interactions;
using Game.PizzeriaSimulator.Interactions.Interactor;
using Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor;
using Game.PizzeriaSimulator.Wallet;
using Game.Root.ServicesInterfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PizzeriaSimulator.PaymentReceive
{
    public class PaymentReceiver : IInittable, ISceneDisposable
    {
        public event Action OnPaymentReceiveEntered;
        public event Action OnPaymentReceiveLeaved;
        public event Action OnStartReceiving;
        public event Action OnReceived;
        readonly Interactor interactor;
        readonly PlayerWallet playerWallet;
        readonly Dictionary<PaymentType, IPaymentProccesor> paymentProccesors;
        public Action currentCallBack;
        bool isReceiving;
        const int maxCentsValue = 99;

        PaymentType targetPaymentType;
        int targetDollars;
        int targetCents;
        public PaymentReceiver(Interactor _interactor, PlayerWallet _playerWallet) 
        {
            interactor = _interactor;
            playerWallet = _playerWallet;
            paymentProccesors = new Dictionary<PaymentType, IPaymentProccesor>()
            {
                { PaymentType.Cash, new CashPaymentProccesor() },
                { PaymentType.Card, new CardPaymentProccesor() }
            };
        }
        public void Init()
        {
            interactor.OnInteract += HandleInteractor;
        }
        public void Dispose()
        {
            interactor.OnInteract -= HandleInteractor;
            paymentProccesors.Clear();

        }
        void HandleInteractor(InteractableType interactableType)
        {
            if(interactableType == InteractableType.PaymentStand)
            {
                EnterPaymentWait();
            }
        }
        public IPaymentProccesor GetPaymentProccesorByType(PaymentType paymentType)
        {
            if(paymentProccesors.TryGetValue(paymentType, out IPaymentProccesor paymentProccesor))
            {
                return paymentProccesor;
            }
            return null;
        }
        public void EnterPaymentWait()
        {
            OnPaymentReceiveEntered?.Invoke();
        }
        public void LeavePaymentInput()
        {
            OnPaymentReceiveLeaved?.Invoke();
        }
      
        public void ProccesPayment(PaymentType paymentType, MoneyQuantity paymentPrice, Action callBack)
        {
            ProccesPayment(paymentType, paymentPrice.Dollars, paymentPrice.Cents, callBack);
        }
        public void ProccesPayment(PaymentType paymentType, int dollars, int cents, Action callBack)
        {
            if (isReceiving)
            {
                callBack?.Invoke();
                return;
            }
            cents = Mathf.Clamp(cents, 0, maxCentsValue);
            if (dollars < 0) dollars = 0;
            targetPaymentType = paymentType;
            targetDollars = dollars;
            targetCents = cents;
            currentCallBack = callBack;
        }
        public void StartReceiveInput()
        {
            if (paymentProccesors.TryGetValue(targetPaymentType, out IPaymentProccesor paymentProccesor))
            {
                isReceiving = true;
                OnStartReceiving?.Invoke();
                paymentProccesor.ProccesPayment(targetDollars, targetCents, OnPaymentReceived);
            }
            else
            {
                UnityEngine.Debug.LogError($"Payment proccesor of type: {targetPaymentType} isn't setted");
            }
        }
        void OnPaymentReceived()
        {
            playerWallet.AddMoney(new MoneyQuantity(targetDollars, targetCents));
            isReceiving = false;
            OnReceived?.Invoke();
            currentCallBack?.Invoke();
        }
    }
}
