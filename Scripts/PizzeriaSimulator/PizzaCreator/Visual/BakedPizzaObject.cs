using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual
{
    public class BakedPizzaObject : MonoBehaviour
    {
        [SerializeField] Transform[] slices;
        public Transform GetPizzaSlice(int sliceNumber)
        {
            if (slices.Length < sliceNumber || sliceNumber < 0) return null;
            return slices[sliceNumber];
        }
    }
}
