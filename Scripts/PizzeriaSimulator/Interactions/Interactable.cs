using UnityEngine;

namespace Game.PizzeriaSimulator.Interactions
{
    public class Interactable : MonoBehaviour
    {
        [field: SerializeField] public InteractableType InteractableType { get; private set; }
        [field: SerializeField] public bool CycledInteract { get; private set; } = false;
        [SerializeField] Outline selectionOutline;
        private void Awake()
        {
            Deselect();
        }
        public virtual void Select()
        {
            if(selectionOutline != null)
            {
                selectionOutline.enabled = true;
            }
        }
        public virtual void Deselect()
        {
            if (selectionOutline != null)
            {
                selectionOutline.enabled = false;
            }
        }
    }
}
