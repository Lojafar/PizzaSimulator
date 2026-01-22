using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Helps.UI
{
    public enum SelectState 
    {
        Normal, Highlighted, Pressed,Selected, Disabled
    }
    public sealed class ExtendedButton : Button
    {
        public event Action OnDownPointer;
        public event Action OnUpPointer;
        public event Action<SelectState> OnSelectStateChanged;
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            OnSelectStateChanged?.Invoke((SelectState)state);
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            OnDownPointer?.Invoke();
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            OnUpPointer?.Invoke();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnDownPointer = null;
            OnUpPointer = null;
            OnSelectStateChanged = null;
        }
    }
}
