using Game.Root.ServicesInterfaces;
using Game.Root.User.Environment;
using R3;
using System;
namespace Game.PizzeriaSimulator.Tutorial.Visual
{
    class TutorialVM : ISceneDisposable
    {
        public event Action ShowEndDayVisual;
        public event Action HideEndDayVisual;
        public event Action<string> UpdateTaskDescription;
        public Subject<Unit> EndDayInput;
        public readonly DeviceType DeviceType;
        readonly TutorialController tutorialController;
        bool endDayTaskActive;
        IDisposable inputDisposable;
        public TutorialVM(TutorialController _tutorialController, DeviceType _deviceType) 
        {
            tutorialController = _tutorialController;
            DeviceType = _deviceType;
            EndDayInput = new Subject<Unit>();
        }
        public void Init()
        {
            tutorialController.OnEndDayTask += HandleEndDayTask;
            tutorialController.OnNewTask += HandleNewTask;
            inputDisposable = EndDayInput.ThrottleFirst(TimeSpan.FromSeconds(0.3f)).Subscribe(_ => tutorialController.EndDayInput());
        }
        public void Dispose()
        {
            inputDisposable.Dispose();
            EndDayInput.Dispose(); 
            tutorialController.OnEndDayTask -= HandleEndDayTask;
            tutorialController.OnNewTask -= HandleNewTask;
        }
        void HandleEndDayTask()
        {
            endDayTaskActive = true;
            ShowEndDayVisual?.Invoke();
            UpdateTaskDescription?.Invoke("No customers will come today. They will start from the next day. Now end the day");
        }
        void HandleNewTask(int taskID)
        {
            if (endDayTaskActive)
            {
                endDayTaskActive = false;
                HideEndDayVisual?.Invoke();
            }
            UpdateTaskDescription?.Invoke($"Task {taskID}");
        }
    }
}
