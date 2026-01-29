using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Tutorial.Visual.SubView
{
    abstract class TutorialSubViewBase : MonoBehaviour
    {
        public event Action EndDayInput;
        public abstract void ActivateEndDayVisual(bool activate);
        protected void RaiseEndDayInput()
        {
            EndDayInput?.Invoke();
        }
        protected virtual void OnDestroy()
        {
            EndDayInput = null;
        }
    }
}
