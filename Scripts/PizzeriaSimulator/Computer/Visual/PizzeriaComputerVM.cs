using Game.PizzeriaSimulator.Computer.App;
using Game.Root.ServicesInterfaces;
using R3;
using System;

namespace Game.PizzeriaSimulator.Computer.Visual
{
    public class PizzeriaComputerVM : ISceneDisposable
    {
        readonly PizzeriaComputer pizzeriaComputer;
        public event Action EnterComputer;
        public event Action ExitComputer;
        public readonly Subject<Unit> ExitPCInput;
        public readonly Subject<Unit> MarketAppInput;
        public readonly Subject<Unit> ManagmentAppInput;
        readonly CompositeDisposable inputSubscriptions;
        public PizzeriaComputerVM(PizzeriaComputer _pizzeriaComputer)
        {
            pizzeriaComputer = _pizzeriaComputer;
            ExitPCInput = new Subject<Unit>();
            MarketAppInput = new Subject<Unit>();
            ManagmentAppInput = new Subject<Unit>();
            inputSubscriptions = new CompositeDisposable();
        }
        public void Init()
        {
            pizzeriaComputer.OnEnterComputer += HandlePCEnter;
            pizzeriaComputer.OnExitComputer += HandlePCExit;
            ExitPCInput.ThrottleFirst(TimeSpan.FromSeconds(0.3f)).Subscribe(_ => pizzeriaComputer.ExitComputer()).AddTo(inputSubscriptions);
            MarketAppInput.ThrottleFirst(TimeSpan.FromSeconds(0.3f)).Subscribe(_ => pizzeriaComputer.OpenApp(ComputerAppType.Market)).AddTo(inputSubscriptions);
            ManagmentAppInput.ThrottleFirst(TimeSpan.FromSeconds(0.3f)).Subscribe(_ => pizzeriaComputer.OpenApp(ComputerAppType.ManagmentApp)).AddTo(inputSubscriptions);
        }
        public void Dispose() 
        {
            pizzeriaComputer.OnEnterComputer -= HandlePCEnter;
            pizzeriaComputer.OnExitComputer -= HandlePCExit;
            inputSubscriptions.Dispose();
            ExitPCInput.Dispose();
            MarketAppInput.Dispose();
            ManagmentAppInput.Dispose();
        }
        void HandlePCEnter()
        {
            EnterComputer?.Invoke();
        }
        void HandlePCExit()
        {
            ExitComputer?.Invoke();
        }
    }
}
