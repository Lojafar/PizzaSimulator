using Cysharp.Threading.Tasks;
using Game.Root.AssetsManagment;
using Game.Root.ServicesInterfaces;
using UnityEngine;
using Zenject;

namespace Game.PizzeriaSimulator.UI.StatusPanel
{
    class StatusPanelBinder
    {
        readonly StatusPanelModel statusPanelModel;
        readonly IAssetsProvider assetsProvider;
        readonly Transform uiParent;
        readonly DiContainer diContainer;
        public StatusPanelBinder(StatusPanelModel _statusPanelModel, IAssetsProvider _assetsProvider, Transform _uiParent, DiContainer _diContainer)
        {
            statusPanelModel = _statusPanelModel;
            assetsProvider = _assetsProvider;
            uiParent = _uiParent;
            diContainer = _diContainer;
        }
        public async UniTask Bind()
        {
            StatusPanelViewBase viewPrefab = await assetsProvider.LoadAsset<StatusPanelViewBase>(AssetsKeys.StatusPanelView);
            StatusPanelViewBase view = Object.Instantiate(viewPrefab, uiParent);
            view.transform.SetAsFirstSibling();
            StatusPanelVM statusPanelVM = new(statusPanelModel);
            view.Bind(statusPanelVM);
            statusPanelVM.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(statusPanelVM);
        }
    }
}
