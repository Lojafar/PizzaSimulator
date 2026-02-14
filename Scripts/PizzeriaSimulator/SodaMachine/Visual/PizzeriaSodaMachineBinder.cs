using Game.Root.ServicesInterfaces;
using Zenject;

namespace Game.PizzeriaSimulator.SodaMachine.Visual
{
    class PizzeriaSodaMachineBinder
    {
        readonly PizzeriaSodaMachine sodaMachine;
        readonly PizzeriaSodaMachineViewBase machineView;
        readonly DiContainer diContainer;
        public PizzeriaSodaMachineBinder(PizzeriaSodaMachine _sodaMachine, PizzeriaSodaMachineViewBase _machineView, DiContainer _diContainer)
        {
            sodaMachine = _sodaMachine;
            machineView = _machineView;
            diContainer = _diContainer;
        }
        public void Bind()
        {
            diContainer.Inject(machineView);
            PizzeriaSodaMachineVM viewModel = new(sodaMachine);
            machineView.Bind(viewModel);
            viewModel.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(viewModel);
        }
    }
}
