using Game.Root.ServicesInterfaces;
using Zenject;
namespace Game.PizzeriaSimulator.Boxes.Carry.Visual
{
    class BoxesCarrierBinder
    {
        readonly BoxesCarrier boxesCarrier;
        readonly DiContainer diContainer;
        public BoxesCarrierBinder(BoxesCarrier _boxesCarrier, DiContainer _diContainer) 
        {
            boxesCarrier = _boxesCarrier;
            diContainer = _diContainer;
        }
        public void Bind()
        {
            IBoxesCarrierView boxesCarrierView = new BoxesCarrierView();
            boxesCarrierView.Bind(boxesCarrier);
            diContainer.Bind<ISceneDisposable>().FromInstance(boxesCarrierView);
        }
    }
}
