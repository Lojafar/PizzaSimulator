using Game.Root.AssetsManagment;
using Game.PizzeriaSimulator.Customers.Skin.Config;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.PizzeriaSimulator.Customers.Skin
{
    class CustomerSkinCreator
    {
        readonly IAssetsProvider assetsProvider;
        CustomerSkinsConfig skinsConfig;
        public CustomerSkinCreator(IAssetsProvider _assetsProvider)
        {
            assetsProvider = _assetsProvider;
        }
        public async UniTask Prewarm()
        {
            skinsConfig = (await assetsProvider.LoadAsset<CustomerSkinsConfigSO>(AssetsKeys.CustomerSkinsConfig)).CustomerSkinsConfig;
        }
        public CustomerSkin CreateSkin(Transform customerTransform)
        {
            CustomerSkin skinPrefab = skinsConfig.GetSkin(Random.Range(0, skinsConfig.SkinsCount));
            return Object.Instantiate(skinPrefab, customerTransform);
        }
    }
}
