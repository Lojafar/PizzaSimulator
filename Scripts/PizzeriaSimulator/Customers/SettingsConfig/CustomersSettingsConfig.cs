using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Customers.SettingsConfig
{
    [Serializable]
    public class CustomersSettingsConfig
    {
        [field: SerializeField] public float MinSpawnDelay { get; private set; }
        [field: SerializeField] public float MaxSpawnDelay { get; private set; }
        [field: SerializeField] public float MaxActiveCustomers { get; private set; }
        [field: SerializeField, Range(0, 100)] public int CardPaymentPercent { get; private set; }
    }
}
