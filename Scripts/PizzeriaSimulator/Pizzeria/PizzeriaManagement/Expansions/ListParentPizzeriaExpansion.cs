using UnityEngine;

namespace Game.PizzeriaSimulator.Pizzeria.Managment.Expansion
{
    sealed class ListParentPizzeriaExpansion : PizzeriaExpansionBase
    {
        [SerializeField] GameObject addingsParent;
        [SerializeField] GameObject[] removings;

        [ContextMenu("Expand")]
        public override void Expand()
        {
            addingsParent.SetActive(true);
            for (int i = 0; i < removings.Length; i++)
            {
                removings[i].SetActive(false);
            }
        }
        [ContextMenu("Collapse")]
        public override void Collapse()
        {
            addingsParent.SetActive(false);
            for (int i = 0; i < removings.Length; i++)
            {
                removings[i].SetActive(true);
            }
        }
    }
}
