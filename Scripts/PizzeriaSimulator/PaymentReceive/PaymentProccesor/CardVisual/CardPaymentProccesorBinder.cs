using Game.Root.ServicesInterfaces;
using Zenject;

namespace Game.PizzeriaSimulator.PaymentReceive.PaymentProccesor.Visual
{
    class CardPaymentProccesorBinder
    {
        readonly PizzeriaSceneReferences sceneReferences;
        readonly CardPaymentProccesor cardPaymentProccesor;
        readonly DiContainer diContainer;
        public CardPaymentProccesorBinder(PizzeriaSceneReferences _sceneReferences, CardPaymentProccesor _cardPaymentProccesor, DiContainer _diContainer)
        {
            sceneReferences = _sceneReferences;
            cardPaymentProccesor = _cardPaymentProccesor;
            diContainer = _diContainer;
        }
        public void Bind()
        {
            CardPaymentProccesorVM cardPaymentProccesorVM = new(cardPaymentProccesor);
            sceneReferences.CardPaymentViewBase.Bind(cardPaymentProccesorVM);
            cardPaymentProccesorVM.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(cardPaymentProccesorVM);
        }
    }
}
