using Game.Root.ServicesInterfaces;
using Zenject;

namespace Game.PizzeriaSimulator.Exit
{
    class PizzariaExitPoint : IInitializable
    {
        readonly PizzeriaSceneReferences sceneReferences;
        readonly DiContainer diContainer;
        public PizzariaExitPoint(PizzeriaSceneReferences _sceneReferences, DiContainer _diContainer)
        {
            sceneReferences = _sceneReferences;
            diContainer = _diContainer;
        }

        public void Initialize()
        {
            sceneReferences.OnLeaveScene += OnShouldBeDisposed;
        }

        void OnShouldBeDisposed()
        {
            sceneReferences.OnLeaveScene -= OnShouldBeDisposed;
            ISceneDisposable[] sceneDisposables = diContainer.Resolve<ISceneDisposable[]>();
            foreach (ISceneDisposable sceneDisposable in sceneDisposables)
            {
                UnityEngine.Debug.Log($"SCENE DISPOSE: {sceneDisposable.GetType()}");
                sceneDisposable.Dispose();
            }
        }


    }
}
