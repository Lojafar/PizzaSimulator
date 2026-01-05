using Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PizzeriaSimulator.PaymentReceive
{
    public class PaymentReceiver
    {
        public event Action OnPaymentReceiveEntered;
        public event Action OnPaymentReceiveLeaved;
        public event Action OnStartReceiving;
        public event Action OnReceived;
        readonly Dictionary<PaymentType, IPaymentProccesor> paymentProccesors;
        public Action currentCallBack;
        bool isReceiving;
        const int maxCentsValue = 99;

        PaymentType targetPaymentType;
        int targetDollars;
        int targetCents;
        public PaymentReceiver() 
        {
            paymentProccesors = new Dictionary<PaymentType, IPaymentProccesor>();
        }
        public void Init()
        {
            paymentProccesors.Add(PaymentType.Cash, new CashPaymentProccesor());
            paymentProccesors.Add(PaymentType.Card, new CardPaymentProccesor());
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
      
        public void ProccesPayment(PaymentType paymentType, PaymentPrice paymentPrice, Action callBack)
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
        public void PaymentReceiveInput()
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
            isReceiving = false;
            OnReceived?.Invoke();
            currentCallBack?.Invoke();
        }
    }
}
