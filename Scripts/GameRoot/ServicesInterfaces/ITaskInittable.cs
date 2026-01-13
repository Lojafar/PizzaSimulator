using Cysharp.Threading.Tasks;

namespace Game.Root.ServicesInterfaces
{
    public interface ITaskInittable
    {
        public UniTask Init();
    }
}
