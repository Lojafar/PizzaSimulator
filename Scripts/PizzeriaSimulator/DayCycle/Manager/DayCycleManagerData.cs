using System;

namespace Game.PizzeriaSimulator.DayCycle.Manager
{
    [Serializable]
    public class DayCycleManagerData
    {
        public int CurrentHour;
        public int CurrentMinutes;
        public bool IsDayPaused;
        public DayCycleManagerData() { }
        public DayCycleManagerData(int _currentHour, int _currentMinutes, bool _isDayPaused) 
        {
            CurrentHour = _currentHour;
            CurrentMinutes = _currentMinutes;
            IsDayPaused = _isDayPaused;
        }
        public DayCycleManagerData Clone()
        {
            return new DayCycleManagerData(CurrentHour, CurrentMinutes, IsDayPaused);
        }
    }
}
