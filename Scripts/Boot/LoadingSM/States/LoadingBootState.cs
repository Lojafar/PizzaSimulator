using UnityEngine;

namespace Game.Boot.LoadingSM.States
{
    class LoadingBootState : INonParamLoadingState
    {
        readonly LoadingStateMachine loadingStateMachine;
        public LoadingBootState(LoadingStateMachine _loadingStateMachine)
        {
            loadingStateMachine = _loadingStateMachine;
        }
        public void Enter()
        {
            Application.targetFrameRate = 0;
           
            loadingStateMachine.EnterState<ServicesPrepareLoadState>();
        }
    }
}
