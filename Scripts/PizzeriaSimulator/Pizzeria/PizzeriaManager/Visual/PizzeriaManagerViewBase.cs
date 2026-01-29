using UnityEngine;

namespace Game.PizzeriaSimulator.Pizzeria.Manager.Visual
{
    public abstract class PizzeriaManagerViewBase : MonoBehaviour
    {
        protected PizzeriaManagerVM managerVM;
        public virtual void Bind(PizzeriaManagerVM _managerVM)
        {
            managerVM = _managerVM;
        }
    }
}
