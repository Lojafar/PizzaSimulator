namespace Game.Boot.LoadingSM.States
{
    public interface IParamLoadingState<T> : ILoadingState
    {
        public void Enter(T param);
    }
}
