using UnityEngine;

namespace Game.Root.Utils.Audio.Config
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Configs/UtilsConfigs/AudioConfig")]
    class AudioConfigSO : ScriptableObject
    {
        [field: SerializeField] public AudioConfig AudioConfig { get; private set; }
    }
}
