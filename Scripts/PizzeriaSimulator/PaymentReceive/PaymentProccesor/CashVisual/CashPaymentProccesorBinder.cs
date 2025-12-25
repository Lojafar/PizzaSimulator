using Game.Root.ServicesInterfaces;
using Zenject;

namespace Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor.Visual
{
    class CashPaymentProccesorBinder
    {
        readonly PizzeriaSceneReferences sceneReferences;
        readonly CashPaymentProccesor cashPaymentProccesor;
        readonly DiContainer diContainer;
        public CashPaymentProccesorBinder(PizzeriaSceneReferences _sceneReferences, CashPaymentProccesor _cashPaymentProccesor, DiContainer _diContainer)
        {
            sceneReferences = _sceneReferences;
            cashPaymentProccesor = _cashPaymentProccesor;
            diContainer = _diContainer;
        }
        public void Bind()
        {
            CashPaymentProccesorVM cashPaymentProccesorVM = new(cashPaymentProccesor);
            sceneReferences.CashPaymentViewBase.Bind(cashPaymentProccesorVM);
            cashPaymentProccesorVM.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(cashPaymentProccesorVM);
        }
    }
}
