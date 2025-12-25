using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaCreation.Visual
{
    abstract class PizzaIngredientContainerBase : MonoBehaviour
    {
        [field: SerializeField] public PizzaIngredientType IngredientType { get; private set; }
        [SerializeField] Outline outline;
        private void Awake()
        {
            Deselect();
        }
        public abstract Vector3 GetDragIngridientPos();
        public abstract GameObject GetObjectForDrag();
        public abstract void RemoveDraggedObject();
        public abstract void CancelDragObject();
        public void Select()
        {
            outline.enabled = true;
        }
        public void Deselect()
        {
            outline.enabled = false;
        }
    }
}
