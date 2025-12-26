using Cysharp.Threading.Tasks;
using Game.Root.AssetsManagment;

namespace Game.Root.User.Environment.Editor
{
    public class EditorEnvironmentHandler : IEnvironmentHandler
    {
        public DeviceType DeviceType { get; private set; }
        readonly IAssetsProvider assetsProvider;
        EditorEnvironmentConfig config;
        public EditorEnvironmentHandler(IAssetsProvider _assetsProvider)
        {
            assetsProvider = _assetsProvider;
        }
        public async UniTask Init()
        {
            config = await assetsProvider.LoadAsset<EditorEnvironmentConfig>(AssetsKeys.EditorEnvironmentConfig);
            DeviceType = config.DeviceType;
        }
    }
}
