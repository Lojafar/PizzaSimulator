using Game.Boot.LoadingSM.States;

namespace Game.Boot.LoadingSM
{
    public interface ILoadingStatesFactory
    {
        public T CreateState<T>() where T : ILoadingState;
    }
}
