using Game.Helps.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.Helps.UI
{
     class ExtendedBtnColorTranstion : MonoBehaviour
    {
        [SerializeField] ExtendedButton extendedButton;
        [SerializeField] Graphic[] graphicsToChange;
        [SerializeField] Color NormalColor = Color.white;
        [SerializeField] Color HighlightedColor = new(0.95f, 0.95f, 0.95f);
        [SerializeField] Color PressedColor = new(0.77f, 0.77f, 0.77f);
        [SerializeField] Color SelectedColor = new(0.97f, 0.97f, 0.97f);
        [SerializeField] Color DisabledColor = new(0.77f, 0.77f, 0.77f, 0.5f);
        private void Awake()
        {
            if (extendedButton == null)
            {
                UnityEngine.Debug.LogError("Extended btn isn't setted!!!");
                return;
            }
            extendedButton.OnSelectStateChanged += HandleBtnSelectState;
        }
        private void OnDestroy()
        {
            extendedButton.OnSelectStateChanged -= HandleBtnSelectState;
        }
        void HandleBtnSelectState(SelectState state)
        {
            Color color = GetColorByState(state);
            for (int i = 0; i < graphicsToChange.Length; i++)
            {
                graphicsToChange[i].color = color;
            }
        }
        Color GetColorByState(SelectState state)
        {
            return state switch
            {
                SelectState.Normal => NormalColor,
                SelectState.Highlighted => HighlightedColor,
                SelectState.Pressed => PressedColor,
                SelectState.Selected => SelectedColor,
                SelectState.Disabled => DisabledColor,
                _ => DisabledColor
            };
        }
    }
}
