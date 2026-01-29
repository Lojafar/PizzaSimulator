using UnityEngine;

namespace Game.PizzeriaSimulator.Tutorial.Visual.SubView
{
    class PCTutorialSubView : TutorialSubViewBase
    {
        [SerializeField] GameObject endDayVisuals;
        [SerializeField] GameObject endDayBtnHint;
        private void Awake()
        {
            endDayVisuals.SetActive(false);
            endDayBtnHint.SetActive(false);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        public override void ActivateEndDayVisual(bool activate)
        {
            endDayVisuals.SetActive(activate);
            endDayBtnHint.SetActive(activate);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                RaiseEndDayInput();
            }
        }
    }
}
