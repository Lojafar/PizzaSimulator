using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Boxes.Item.Consumable
{
    public abstract class ConsumableBoxItemBase : MonoBehaviour
    {
        public abstract void SetTo(Vector3 pos, Vector3 eulerRotatiton, Action onCompleted = null);
    }
}
