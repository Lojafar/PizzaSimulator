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

        const int maxCentsValue = 99;
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
        public void EnterPaymentReceive()
        {
            OnPaymentReceiveEntered?.Invoke();
        }
        public void LeavePaymentReceive()
        {
            OnPaymentReceiveLeaved?.Invoke();
        }
        public void ReceivePayment(PaymentType paymentType, int dollars, int cents)
        {
            cents = Mathf.Clamp(cents, 0, maxCentsValue);
            if (dollars < 0) dollars = 0;

            if (paymentProccesors.TryGetValue(paymentType, out IPaymentProccesor paymentProccesor))
            {
                OnStartReceiving?.Invoke();
                paymentProccesor.ProccesPayment(dollars, cents, OnPaymentReceived);
            }
            else
            {
                UnityEngine.Debug.LogError($"Payment proccesor of type: {paymentType} isn't setted");
            }

        }
        void OnPaymentReceived()
        {
            OnReceived?.Invoke();
        }
    }
}
