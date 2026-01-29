using Cysharp.Threading.Tasks;
using Game.Root.AssetsManagment;
using Game.Root.ServicesInterfaces;
using UnityEngine;
using Zenject;

namespace Game.PizzeriaSimulator.Tutorial.Visual
{
    using DeviceType = Game.Root.User.Environment.DeviceType;
    class TutorialVisualBinder
    {
        readonly IAssetsProvider assetsProvider;
        readonly TutorialController tutorialController;
        readonly Transform visualParent;
        readonly DeviceType deviceType;
        readonly DiContainer diContainer;
        public TutorialVisualBinder(IAssetsProvider _assetsProvider, TutorialController _tutorialController, Transform _visualParent, DeviceType _deviceType, DiContainer _diContainer)
        {
            assetsProvider = _assetsProvider;
            tutorialController = _tutorialController;
            visualParent = _visualParent;
            deviceType = _deviceType;
            diContainer = _diContainer;
        }
        public async UniTask Bind()
        {
            TutorialViewBase tutorialViewPrefab = await assetsProvider.LoadAsset<TutorialViewBase>(AssetsKeys.TutorialView);
            TutorialViewBase spawnedView = Object.Instantiate(tutorialViewPrefab, visualParent);
            TutorialVM tutorialVM = new(tutorialController, deviceType);
            spawnedView.Bind(tutorialVM);
            tutorialVM.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(tutorialVM);
        }
    }
}
