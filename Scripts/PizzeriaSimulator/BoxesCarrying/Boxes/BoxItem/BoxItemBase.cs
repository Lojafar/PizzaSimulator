using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.BoxCarry.Box.Item
{
    public abstract class BoxItemBase : MonoBehaviour
    {
        public abstract void SetTo(Vector3 pos, Action onCompleted = null);
    }
}
