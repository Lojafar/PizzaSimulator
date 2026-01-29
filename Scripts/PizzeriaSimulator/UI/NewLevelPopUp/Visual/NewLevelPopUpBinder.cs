using Cysharp.Threading.Tasks;
using Game.Root.AssetsManagment;
using Game.Root.ServicesInterfaces;
using UnityEngine;
using Zenject;

namespace Assets.Game.Scripts.PizzeriaSimulator.UI.NewLevelPopUp.Visual
{
    class NewLevelPopUpBinder
    {
        readonly NewLevelPopUpModel newLevelPopUpModel;
        readonly IAssetsProvider assetsProvider;
        readonly Transform uiParent;
        readonly DiContainer diContainer;
        public NewLevelPopUpBinder(NewLevelPopUpModel _newLevelPopUpModel, IAssetsProvider _assetsProvider, Transform _uiParent, DiContainer _diContainer) 
        {
            newLevelPopUpModel = _newLevelPopUpModel;
            assetsProvider = _assetsProvider;
            uiParent = _uiParent;
            diContainer = _diContainer;
        }
        public async UniTask Bind()
        {
            NewLevelPopUpViewBase viewPrefab = await assetsProvider.LoadAsset<NewLevelPopUpViewBase>(AssetsKeys.NewLevelPopUpView);
            NewLevelPopUpViewBase view = Object.Instantiate(viewPrefab, uiParent);
            NewLevelPopUpVM newLevelPopUpVM = new NewLevelPopUpVM(newLevelPopUpModel);
            view.Bind(newLevelPopUpVM);
            newLevelPopUpVM.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(newLevelPopUpVM);
        }
    }
}
