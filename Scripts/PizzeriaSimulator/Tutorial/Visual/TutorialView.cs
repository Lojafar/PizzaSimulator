using Game.PizzeriaSimulator.Tutorial.Visual.SubView;
using TMPro;
using UnityEngine;
using R3;

namespace Game.PizzeriaSimulator.Tutorial.Visual
{
    class TutorialView : TutorialViewBase
    {
        [SerializeField] TMP_Text taskDescriptionTMP;
        [SerializeField] TutorialSubViewBase pcSubView;
        [SerializeField] TutorialSubViewBase mobileSubView;

        TutorialSubViewBase currentSubView;
        public override void Bind(TutorialVM _viewModel)
        {
            base.Bind(_viewModel);
            viewModel.UpdateTaskDescription += UpdateTaskDescription;
            viewModel.ShowEndDayVisual += ShowEndDayVisuals;
            viewModel.HideEndDayVisual += HideEndDayVisuals;
            if (viewModel.DeviceType == Root.User.Environment.DeviceType.PC)
            {
                Destroy(mobileSubView);
                currentSubView = pcSubView;
            }
            else
            {
                Destroy(pcSubView);
                currentSubView = mobileSubView;
            }
            currentSubView.EndDayInput += OnEnDayInput;
        }
        private void OnDestroy()
        {
            viewModel.UpdateTaskDescription -= UpdateTaskDescription;
            viewModel.ShowEndDayVisual -= ShowEndDayVisuals;
            viewModel.HideEndDayVisual -= HideEndDayVisuals;
            currentSubView.EndDayInput -= OnEnDayInput;
        }
        void UpdateTaskDescription(string text)
        {
            taskDescriptionTMP.text = text;
        }
        void ShowEndDayVisuals()
        {
            currentSubView.ActivateEndDayVisual(true);
        }
        void HideEndDayVisuals()
        {
            currentSubView.ActivateEndDayVisual(false);
        }
        void OnEnDayInput()
        {
            viewModel.EndDayInput.OnNext(Unit.Default);
        }
    }
}
