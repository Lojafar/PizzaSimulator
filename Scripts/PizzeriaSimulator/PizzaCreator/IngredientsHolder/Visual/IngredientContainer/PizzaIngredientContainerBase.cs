using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaCreation.IngredientsHold.Visual
{
    abstract class PizzaIngredientContainerBase : MonoBehaviour
    {
        [field: SerializeField] public PizzaIngredientType IngredientType { get; private set; }
        [SerializeField] Outline outline;
        private void Awake()
        {
            Deselect();
        }
        public abstract void AddIngredient(GameObject gameObject);
        public abstract void AddIngredient();
        public abstract void RemoveIngredient();
        public abstract Vector3 GetDragIngredientPos();
        public abstract Vector3 GetNextIngredientPos();
        public abstract GameObject GetObjectForDrag();
        public abstract void EndDragObject();
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
