using System;
namespace Game.Root.SceneManagment
{
    public interface IScenesLoader
    {
        public event Action OnStartLoadScene;
        public event Action<float> OnSceneLoadPercentChanged;
        public event Action OnSceneLoaded;
        public void LoadScene(string sceneName);
    }
}
