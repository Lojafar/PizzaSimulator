namespace Game.Root.ServicesInterfaces
{
    public interface IInittable
    {
        public int InitPriority { get; }
        public void Init();
    }
}
