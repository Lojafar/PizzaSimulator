using UnityEngine;

namespace Game.PizzeriaSimulator.Pizzeria.Managment.Expansion
{
    public abstract class PizzeriaExpansionBase : MonoBehaviour
    {
        [field: SerializeField] public ExpansionFurnPlaceOverride[] ExpansionFurnPlaceOverrides { get; protected set; }
        protected virtual void Awake()
        {
            Collapse();
        }
        public abstract void Expand();
        public abstract void Collapse();
    }
}
