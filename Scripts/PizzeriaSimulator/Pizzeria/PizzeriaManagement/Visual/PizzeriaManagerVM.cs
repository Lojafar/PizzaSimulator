using Game.Root.ServicesInterfaces;
using System;
using R3;
namespace Game.PizzeriaSimulator.Pizzeria.Managment.Visual
{
    public sealed class PizzeriaManagerVM : ISceneDisposable
    {
        public event Action<bool> ClosePizzeria;
        public event Action<bool> OpenPizzeria;
        readonly PizzeriaManager pizzeriaManager;
        IDisposable openSubscription;
        public PizzeriaManagerVM(PizzeriaManager _pizzeriaManager)
        {
            pizzeriaManager = _pizzeriaManager;
        }
        public void Init()
        {
            openSubscription = pizzeriaManager.Opened.Subscribe(HandlePizzeriaOpen);
        }
        public void Dispose()
        {
            openSubscription.Dispose();
        }
        void HandlePizzeriaOpen(bool opened)
        {
            if (opened) OpenPizzeria?.Invoke(pizzeriaManager.LastOpenImmediately);
            else ClosePizzeria?.Invoke(pizzeriaManager.LastOpenImmediately);
        }
    }
}
