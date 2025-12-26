using Cysharp.Threading.Tasks;

namespace Game.Root.User.Environment
{
    public interface IEnvironmentHandler
    {
        public DeviceType DeviceType { get; }
        public UniTask Init();
    }
}
