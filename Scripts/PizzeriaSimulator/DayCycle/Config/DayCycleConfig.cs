using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.DayCycle.Config
{
    [Serializable]
    public class DayCycleConfig
    {
        [field: SerializeField] public float RealSecondsMultiplier { get; private set; }
        [field: SerializeField] public int StartHour { get; private set; }
        [field: SerializeField] public int EndHour { get; private set; }
        [field: SerializeField] public Gradient SkyColorGradient { get; private set; }
        [field: SerializeField] public Gradient AmbientColorGradient { get; private set; }
        [field: SerializeField] public Gradient DirectionColorGradient { get; private set; }
    }
}
