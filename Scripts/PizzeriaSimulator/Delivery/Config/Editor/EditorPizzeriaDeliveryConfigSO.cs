using UnityEngine;
using UnityEditor;
using System.Reflection;
namespace Game.PizzeriaSimulator.Delivery.Config.Editor
{
    [CustomEditor(typeof(PizzeriaDeliveryConfigSO))]
    class EditorPizzeriaDeliveryConfigSO : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PizzeriaDeliveryConfigSO scriptTarget = (PizzeriaDeliveryConfigSO)target;
            if (GUILayout.Button("UPDATE ITEMS ID"))
            {
                PropertyInfo idField = typeof(DeliveryItemConfig).GetProperty("ID", BindingFlags.Instance | BindingFlags.Public);
                for (int i = 0; i < scriptTarget.DeliveryConfig.ItemsAmount; i++) 
                {
                    idField.SetValue(scriptTarget.DeliveryConfig.GetDeliveryItemConfig(i), i);
                }
            }
        }
    }
}
