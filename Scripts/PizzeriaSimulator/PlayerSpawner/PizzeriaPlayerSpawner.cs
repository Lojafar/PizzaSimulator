using Game.Root.AssetsManagment;
using Game.PizzeriaSimulator.Player;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Game.PizzeriaSimulator.Player.Input;
using Game.PizzeriaSimulator.Player.CameraController;
namespace Game.PizzeriaSimulator.PlayerSpawner
{
    public class PizzeriaPlayerSpawner
    {
        readonly IPlayerInput playerInput;
        readonly PizzeriaSceneReferences sceneReferences;
        readonly IAssetsProvider assetsProvider;
        readonly DiContainer diContainer;
        public PizzeriaPlayerSpawner(IPlayerInput _playerInput, PizzeriaSceneReferences _sceneReferences, IAssetsProvider _assetsProvider, DiContainer _diContainer)
        {
            playerInput = _playerInput;
            sceneReferences = _sceneReferences;
            assetsProvider = _assetsProvider;
            diContainer = _diContainer;
        }
        public async UniTask<PizzaPlayer> SpawnPlayer() 
        {
            PizzaPlayer playerPrefab = await assetsProvider.LoadAsset<PizzaPlayer>(AssetsKeys.PlayerPrefab);
            PizzaPlayer spawnedPlayer = Object.Instantiate(playerPrefab, sceneReferences.PlayerSpawnPoint.position, sceneReferences.PlayerSpawnPoint.rotation);

            PlayerCameraControllerBase playerCamPrefab = await assetsProvider.LoadAsset<PlayerCameraControllerBase>(AssetsKeys.PlayerCameraPrefab);
            PlayerCameraControllerBase spawnedPlayerCam = Object.Instantiate(playerCamPrefab, spawnedPlayer.EyesTransform);

            spawnedPlayerCam.Construct(spawnedPlayer);
            spawnedPlayer.Construct(playerInput, spawnedPlayerCam);
            diContainer.Bind<PlayerCameraControllerBase>().FromInstance(spawnedPlayerCam).AsSingle();
            diContainer.Bind<PizzaPlayer>().FromInstance(spawnedPlayer).AsSingle();
            playerInput.Activate(true);
            return spawnedPlayer;
        }
    }
}
