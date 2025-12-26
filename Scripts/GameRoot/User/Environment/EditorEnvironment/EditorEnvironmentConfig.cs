using UnityEngine;

namespace Game.Root.User.Environment.Editor
{
    [CreateAssetMenu(fileName = "EditorEnvironmentConfig", menuName = "Configs/Root/EditorEnvironment")]
    public class EditorEnvironmentConfig : ScriptableObject
    {
        [field: SerializeField] public DeviceType DeviceType { get; private set; }
    }
}
