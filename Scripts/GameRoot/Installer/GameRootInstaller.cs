using Game.Root.AssetsManagment;
using Game.Root.SceneManagment;
using Game.Root.Utils;
using UnityEngine;
using Zenject;

namespace Game.Root.Installer
{
    class GameRootInstaller : MonoInstaller
    {
        private void Awake()
        {
#if UNITY_EDITOR
            if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != Scenes.BootSceneName)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(Scenes.BootSceneName);
            }
#endif
        }
        public override void InstallBindings()
        {
            Coroutines coroutines = new GameObject("COROUTINES").AddComponent<Coroutines>();
            DontDestroyOnLoad(coroutines.gameObject);
            Container.Bind<Coroutines>().FromInstance(coroutines).AsSingle();
            BindServices();
        }
        void BindServices()
        {
            Container.BindInterfacesAndSelfTo<ResourcesAssetsProvider>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AsyncScenesLoader>().AsSingle().NonLazy();
        }
    }
}
