using Game.Root.ServicesInterfaces;

namespace Game.PizzeriaSimulator.Boxes.Carry.Visual
{
    interface IBoxesCarrierView : ISceneDisposable
    {
        public void Bind(BoxesCarrier boxesCarrier);
    }
}
