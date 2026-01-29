using UnityEngine;
using UnityEngine.UI;
namespace Game.PizzeriaSimulator.Tutorial.Visual.SubView
{
    class MobileTutorialSubView : TutorialSubViewBase
    {
        [SerializeField] Button endDayButton;
        [SerializeField] GameObject endDayVisuals;
        private void Awake()
        {
            endDayButton.onClick.AddListener(RaiseEndDayInput);
            endDayVisuals.SetActive(false);
        }
        protected override void OnDestroy()
        {
            endDayButton.onClick.RemoveListener(RaiseEndDayInput);
            base.OnDestroy();
        }
        public override void ActivateEndDayVisual(bool activate)
        {
            endDayVisuals.SetActive(activate);
        }
    }
}
