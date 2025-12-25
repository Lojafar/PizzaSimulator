using Game.Root.SceneManagment;

namespace Game.Boot.LoadingSM.States
{
    class GameLoadingState : INonParamLoadingState
    {
        readonly IScenesLoader scenesLoader;
        public GameLoadingState(IScenesLoader _scenesLoader)
        {
            scenesLoader = _scenesLoader;
        }
        public void Enter()
        {
            scenesLoader.LoadScene(Scenes.PizzeriaSceneName);
        }
    }
}
