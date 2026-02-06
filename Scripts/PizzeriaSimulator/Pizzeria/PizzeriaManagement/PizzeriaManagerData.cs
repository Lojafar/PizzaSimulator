using System;
namespace Game.PizzeriaSimulator.Pizzeria.Managment
{
    [Serializable]
    public sealed class PizzeriaManagerData 
    {
        public int CurrentLevel;
        public int CurrentLvlXP;
        public int UnlockedExpansionsMask;
        public bool Opened;
        public bool FirstOpenOfDay;
        public PizzeriaManagerData()
        {
            
        }
        public PizzeriaManagerData(int _currentLevel, int _currentLvlXP, int _unlockedExpansionsMask, bool _opened, bool _firstOpenOfDay)
        {
            CurrentLevel = _currentLevel;
            CurrentLvlXP = _currentLvlXP;
            UnlockedExpansionsMask = _unlockedExpansionsMask;
            Opened = _opened;
            FirstOpenOfDay = _firstOpenOfDay;
        }
        public PizzeriaManagerData Clone()
        {
            return new PizzeriaManagerData(CurrentLevel, CurrentLvlXP, UnlockedExpansionsMask, Opened, FirstOpenOfDay);
        }
    }
}
