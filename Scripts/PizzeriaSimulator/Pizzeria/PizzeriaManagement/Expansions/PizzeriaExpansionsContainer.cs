using UnityEngine;

namespace Game.PizzeriaSimulator.Pizzeria.Managment.Expansion
{
    public sealed class PizzeriaExpansionsContainer : MonoBehaviour
    {
        [SerializeField] PizzeriaExpansionBase[] expansionsInOrder;
        public PizzeriaExpansionBase GetExpansion(int id)
        {
            if (id > -1 && id < expansionsInOrder.Length) return expansionsInOrder[id];
            return null;
        }
    }
}
