using UnityEditor;
using UnityEngine;

namespace Game.Helps.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
