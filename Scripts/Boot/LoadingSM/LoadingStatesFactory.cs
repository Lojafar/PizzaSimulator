using Game.Boot.LoadingSM.States;
using Zenject;

namespace Game.Boot.LoadingSM
{ 
    class LoadingStatesFactory : ILoadingStatesFactory
    {
        readonly DiContainer container;
        public LoadingStatesFactory(DiContainer _container)
        {
            container = _container;
        }
        public T CreateState<T>() where T : ILoadingState
        {
            return container.Resolve<T>();
        }
    }
}