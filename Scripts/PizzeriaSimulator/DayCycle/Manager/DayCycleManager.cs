using Game.PizzeriaSimulator.DayCycle.Config;
using Game.Root.ServicesInterfaces;
using Game.Root.Utils;
using System;
using UnityEngine;
using R3;
namespace Game.PizzeriaSimulator.DayCycle.Manager
{
    public class DayCycleManager : IInittable, ISceneDisposable
    {
        public int InitPriority => 15;
        public event Action OnDayStarted;
        public event Action OnDayEnded;
        public ReadOnlyReactiveProperty<bool> IsDayPaused => isDayPaused;
        public ReadOnlyReactiveProperty<int> Hours => hours;
        public ReadOnlyReactiveProperty<int> Minutes => minutes;
        public bool IsDayEnded { get; private set; }
        readonly ReactiveProperty<bool> isDayPaused;
        readonly ReactiveProperty<int> hours;
        readonly ReactiveProperty<int> minutes;
        readonly DayCycleManagerData managerData;
        readonly float timeMultiplier;
        readonly int startHour;
        readonly int endHour;
        float floatMinutes;
        const float maxMinutes = 59;
        public DayCycleManager(DayCycleManagerData _managerData, DayCycleConfig _dayCycleConfig)
        {
            timeMultiplier = _dayCycleConfig.RealSecondsMultiplier;
            startHour = _dayCycleConfig.StartHour;
            endHour = _dayCycleConfig.EndHour;
            managerData = _managerData ?? new DayCycleManagerData(startHour, 0, true);
            isDayPaused = new ReactiveProperty<bool>(managerData.IsDayPaused);
            hours = new ReactiveProperty<int>(managerData.CurrentHour);
            minutes = new ReactiveProperty<int>(managerData.CurrentMinutes);
        }
        public DayCycleManagerData GetManagerData()
        {
            managerData.IsDayPaused = isDayPaused.CurrentValue;
            managerData.CurrentMinutes = minutes.CurrentValue;
            managerData.CurrentHour = hours.CurrentValue;
            return managerData.Clone();
        }
        public void Init()
        {
            if (hours.CurrentValue >= endHour)
            {
                IsDayEnded = true;
                OnDayEnded?.Invoke();
            }
            floatMinutes = minutes.CurrentValue;
            Ticks.Instance.OnTick += OnTick;
        }
        public void Dispose() 
        {
            Ticks.Instance.OnTick -= OnTick;
        }
        void OnTick()
        {
            if (IsDayEnded || isDayPaused.CurrentValue) return;
            floatMinutes += Time.deltaTime * timeMultiplier;
            if ((int)floatMinutes != minutes.Value)
            {
                if (minutes.Value > maxMinutes)
                {
                    NextHour();
                }
                else
                {
                    minutes.Value = (int)floatMinutes;
                }
            }
        }
        void NextHour()
        {
            floatMinutes = 0;
            hours.Value++;
            minutes.Value = 0;
            if (hours.Value >= endHour)
            {
                IsDayEnded = true;
                OnDayEnded?.Invoke();
            }
        }
        public void RestartDay()
        {
            isDayPaused.Value = true;
            floatMinutes = 0;
            if (hours.Value == startHour)
            {
                hours.OnNext(startHour);
            }
            else
            {
                hours.Value = startHour;
            }
            if (minutes.Value == 0)
            {
                minutes.OnNext(0);
            }
            else
            {
                minutes.Value = 0;
            }
            IsDayEnded = false;
            OnDayStarted?.Invoke();
        }
        public void PauseDay()
        {
            isDayPaused.Value = true;
        }
        public void ResumeDay()
        {
            isDayPaused.Value = false;
        }
    }
}
