using Game.Root.ServicesInterfaces;
using Zenject;

namespace Game.PizzeriaSimulator.Computer.Visual
{
    class PizzeriaComputerBinder
    {
        readonly PizzeriaComputer pizzeriaComputer;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly DiContainer diContainer;
        public PizzeriaComputerBinder(PizzeriaComputer _pizzeriaComputer, PizzeriaSceneReferences _sceneReferences, DiContainer _diContainer)
        {
            pizzeriaComputer = _pizzeriaComputer;
            sceneReferences = _sceneReferences;
            diContainer = _diContainer;
        }
        public void Bind()
        {
            PizzeriaComputerVM computerVM = new(pizzeriaComputer);
            sceneReferences.PizzeriaComputerView.Bind(computerVM);
            computerVM.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(computerVM);
        }
    }
}
