using Game.Root.ServicesInterfaces;
using Zenject;

namespace Game.PizzeriaSimulator.PaymentReceive.Visual
{
    class PaymentReceiveBinder
    {
        readonly PizzeriaSceneReferences sceneReferences;
        readonly PaymentReceiver paymentReceiver;
        readonly DiContainer diContainer;
        public PaymentReceiveBinder(PizzeriaSceneReferences _sceneReferences, PaymentReceiver _paymentReceiver, DiContainer _diContainer)
        {
            sceneReferences = _sceneReferences;
            paymentReceiver = _paymentReceiver;
            diContainer = _diContainer;
        }
        public void Bind()
        {
            PaymentReceiverVM paymentReceiverVM = new(paymentReceiver);
            sceneReferences.PaymentReceiverViewBase.Bind(paymentReceiverVM);
            paymentReceiverVM.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(paymentReceiverVM);
        }
    }
}
