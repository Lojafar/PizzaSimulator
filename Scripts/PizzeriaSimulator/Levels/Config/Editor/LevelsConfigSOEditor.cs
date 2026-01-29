using Game.PizzeriaSimulator.Leevl.Reward;
using UnityEditor;
using UnityEngine;

namespace Game.PizzeriaSimulator.Levels.Config.Editor
{
    using Editor = UnityEditor.Editor;
    [CustomEditor(typeof(LevelsConfigSO))]
    class LevelsConfigSOEditor : Editor
    {
        LevelRewardType selectedReward;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            selectedReward = (LevelRewardType)EditorGUILayout.EnumPopup("REWARD TO ADD", selectedReward);

            if(GUILayout.Button("ADD REWARD"))
            {
                ((LevelsConfigSO)target).AddRewardToConfig(selectedReward);
            }
        }
    }
}
