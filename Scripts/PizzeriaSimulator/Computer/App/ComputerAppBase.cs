using System;

namespace Game.PizzeriaSimulator.Computer.App
{
    public abstract class ComputerAppBase
    {
        public virtual event Action OnOpen;
        public virtual event Action OnClose;
        public virtual void Open()
        {
            OnOpen?.Invoke();
        }
        public virtual void Close()
        {
            OnClose?.Invoke();
        }
    }
}
