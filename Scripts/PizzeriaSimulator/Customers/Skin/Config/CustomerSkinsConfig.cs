using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Customers.Skin.Config
{
    [Serializable]
    public class CustomerSkinsConfig
    {
        [SerializeField] CustomerSkin[] skins;
        public int SkinsCount => skins.Length;
        public CustomerSkin GetSkin(int index)
        {
            if (index < 0 || index >= skins.Length)
            {
                Debug.Log($"Index of customer skin isn't correct. Index is: {index}");
                return null;
            }
            return skins[index];
        }
    }
}
