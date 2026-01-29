using System;

namespace Game.PizzeriaSimulator.Pizzeria.Manager
{
    [Serializable]
    public class PizzeriaManagerData 
    {
        public int CurrentLevel;
        public int CurrentLvlXP;
        public bool Opened;
        public bool FirstOpenOfDay;
        public PizzeriaManagerData()
        {

        }
        public PizzeriaManagerData(int _currentLevel, int _currentLvlXP, bool _opened, bool _firstOpenOfDay)
        {
            CurrentLevel = _currentLevel;
            CurrentLvlXP = _currentLvlXP;
            Opened = _opened;
            FirstOpenOfDay = _firstOpenOfDay;
        }
        public PizzeriaManagerData Clone()
        {
            return new PizzeriaManagerData(CurrentLevel, CurrentLvlXP, Opened, FirstOpenOfDay);
        }
    }
}
