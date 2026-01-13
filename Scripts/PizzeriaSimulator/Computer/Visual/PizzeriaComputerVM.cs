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
        public PizzeriaComputerVM(PizzeriaComputer _pizzeriaComputer)
        {
            pizzeriaComputer = _pizzeriaComputer;
            ExitPCInput = new Subject<Unit>();
            MarketAppInput = new Subject<Unit>();
        }
        public void Init()
        {
            pizzeriaComputer.OnEnterComputer += HandlePCEnter;
            pizzeriaComputer.OnExitComputer += HandlePCExit;
            ExitPCInput.ThrottleFirst(TimeSpan.FromSeconds(0.3f)).Subscribe(_ => pizzeriaComputer.ExitComputer());
            MarketAppInput.ThrottleFirst(TimeSpan.FromSeconds(0.3f)).Subscribe(_ => pizzeriaComputer.OpenApp(ComputerAppType.Market));
        }
        public void Dispose() 
        {
            pizzeriaComputer.OnEnterComputer -= HandlePCEnter;
            pizzeriaComputer.OnExitComputer -= HandlePCExit;
            ExitPCInput.Dispose();
            MarketAppInput.Dispose();
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
