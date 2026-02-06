using System;
using UnityEngine;

namespace Game.PizzeriaSimulator.Boxes.Item
{
    public abstract class PizzaIngredientBoxItemBase : MonoBehaviour
    {
        public abstract void SetTo(Vector3 pos, Action onCompleted = null);
    }
}
