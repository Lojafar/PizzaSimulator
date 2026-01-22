using Game.Root.ServicesInterfaces;
using Game.Root.User.Environment;
using Zenject;

namespace Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor.Visual
{

    class CashPaymentProccesorBinder
    {
        readonly PizzeriaSceneReferences sceneReferences;
        readonly CashPaymentProccesor cashPaymentProccesor;
        readonly DeviceType deviceType;
        readonly DiContainer diContainer;
        public CashPaymentProccesorBinder(PizzeriaSceneReferences _sceneReferences, CashPaymentProccesor _cashPaymentProccesor, DeviceType _deviceType, DiContainer _diContainer)
        {
            sceneReferences = _sceneReferences;
            cashPaymentProccesor = _cashPaymentProccesor;
            deviceType = _deviceType;
            diContainer = _diContainer;
        }
        public void Bind()
        {
            CashPaymentProccesorVM cashPaymentProccesorVM = new(cashPaymentProccesor, deviceType);
            sceneReferences.CashPaymentViewBase.Bind(cashPaymentProccesorVM);
            cashPaymentProccesorVM.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(cashPaymentProccesorVM);
        }
    }
}
