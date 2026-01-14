using System;
using Game.Root.ServicesInterfaces;
using R3;

namespace Game.PizzeriaSimulator.PaymentReceive.Visual
{
    public class PaymentReceiverVM : ISceneDisposable
    {
        public event Action OnEnterReceive;
        public event Action OnLeaveReceive;
        public event Action OnStartReceiving;
        public event Action OnReceived;

        public Subject<Unit> LeaveInput;
        public Subject<Unit> PaymentReceiveInput;

        readonly PaymentReceiver paymentReceiver;
        public PaymentReceiverVM(PaymentReceiver _paymentReceiver)
        {
            paymentReceiver = _paymentReceiver;
            LeaveInput = new Subject<Unit>();
            PaymentReceiveInput = new Subject<Unit>();
        }
        public void Init()
        {
            LeaveInput.ThrottleFirst(TimeSpan.FromSeconds(0.1f)).Subscribe(_ =>paymentReceiver.LeavePaymentInput());
            PaymentReceiveInput.ThrottleFirst(TimeSpan.FromSeconds(0.1f)).Subscribe(_ =>paymentReceiver.StartReceiveInput());
            paymentReceiver.OnPaymentReceiveEntered += OnEnterPaymentReceive;
            paymentReceiver.OnPaymentReceiveLeaved += OnLeavePaymentReceive;
            paymentReceiver.OnStartReceiving += OnReceiveStart;
            paymentReceiver.OnReceived += OnReceiveEnd;
        }
        public void Dispose()
        {
            LeaveInput.Dispose();
            PaymentReceiveInput.Dispose();
            paymentReceiver.OnPaymentReceiveEntered -= OnEnterPaymentReceive;
            paymentReceiver.OnPaymentReceiveLeaved -= OnLeavePaymentReceive;
            paymentReceiver.OnStartReceiving -= OnReceiveStart;
            paymentReceiver.OnReceived -= OnReceiveEnd;
        }
        void OnEnterPaymentReceive()
        {
            OnEnterReceive?.Invoke();
        }
        void OnLeavePaymentReceive()
        {
            OnLeaveReceive?.Invoke();
        }
        void OnReceiveStart()
        {
            OnStartReceiving?.Invoke();
        }
        void OnReceiveEnd()
        {
            OnReceived?.Invoke();
        }
    }
}
