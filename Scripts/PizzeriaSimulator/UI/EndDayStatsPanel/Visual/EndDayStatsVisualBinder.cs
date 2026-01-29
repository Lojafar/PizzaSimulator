using Game.Root.AssetsManagment;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;
using Game.Root.ServicesInterfaces;

namespace Game.PizzeriaSimulator.UI.EndDayStatsPanel.Visual
{
    class EndDayStatsVisualBinder
    {
        readonly EndDayStatsModel endDayStatsModel;
        readonly IAssetsProvider assetsProvider;
        readonly Transform uiParent;
        readonly DiContainer diContainer;
        public EndDayStatsVisualBinder(EndDayStatsModel _endDayStatsModel, IAssetsProvider _assetsProvier, Transform _uiParent, DiContainer _diContainer) 
        {
            endDayStatsModel = _endDayStatsModel;
            assetsProvider = _assetsProvier;
            uiParent = _uiParent;
            diContainer = _diContainer;
        }
        public async UniTask Bind()
        {
            EndDayStatsViewBase viewPrefab = await assetsProvider.LoadAsset<EndDayStatsViewBase>(AssetsKeys.EndDayResultView);
            EndDayStatsViewBase spawnedView = Object.Instantiate(viewPrefab, uiParent);
            EndDayStatsVM viewModel = new(endDayStatsModel);
            spawnedView.Bind(viewModel);
            viewModel.Init();
            diContainer.Bind<ISceneDisposable>().FromInstance(viewModel);
        }
    }
}
