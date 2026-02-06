using System;
using UnityEngine;
namespace Game.PizzeriaSimulator.Pizzeria.Managment.Config
{
    [Serializable]
    public sealed class PizzeriaManagmentConfig
    {
        [SerializeField] PizzeriaExpansionConfig[] expansions;
        public int ExpansionsCount => expansions.Length;
        public PizzeriaExpansionConfig GetExpansionConfig(int id)
        {
            if(id < 0 || id >= ExpansionsCount) return null;
            return expansions[id];
        }
    }
}
