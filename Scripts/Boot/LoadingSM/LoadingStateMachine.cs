using Game.Boot.LoadingSM.States;
using System;
using System.Collections.Generic;
using Zenject;
namespace Game.Boot.LoadingSM
{
    public class LoadingStateMachine : IInitializable
    {
        readonly ILoadingStatesFactory loadingStatesFactory;
        readonly Dictionary<Type, ILoadingState> statesMap;
        public LoadingStateMachine(ILoadingStatesFactory _loadingStatesFactory)
        {
            loadingStatesFactory = _loadingStatesFactory;
            statesMap = new();
        }
        public void Initialize()
        {
            statesMap.Add(typeof(LoadingBootState), loadingStatesFactory.CreateState<LoadingBootState>());
            statesMap.Add(typeof(ServicesPrepareLoadState), loadingStatesFactory.CreateState<ServicesPrepareLoadState>());
            statesMap.Add(typeof(GameLoadingState), loadingStatesFactory.CreateState<GameLoadingState>());
            UnityEngine.Debug.Log("LOADING STATE MACHINE");
            EnterState<LoadingBootState>();
        }
        public void EnterState<T>() where T : class, INonParamLoadingState
        {
            ChangeState<T>().Enter();
        }
        public void EnterState<TState, TParam>(TParam param) where TState : class, IParamLoadingState<TParam>
        {
            ChangeState<TState>().Enter(param);
        }
        T ChangeState<T>() where T : class, ILoadingState
        {
            UnityEngine.Debug.Log($"LSM state changed to {typeof(T)}");
            return GetState<T>() as T;
        }
        T GetState<T>() where T : class, ILoadingState
        {
            return statesMap[typeof(T)] as T;
        }
    }
}
