using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Root.User.Environment.Editor
{
    public class BuildEnvironmentHandler : IEnvironmentHandler
    {
        public DeviceType DeviceType { get; private set; }
        public async UniTask Init()
        {
            await UniTask.Yield();
            DeviceType = Application.isMobilePlatform ? DeviceType.Mobile : DeviceType.PC;
        }
    }
}
