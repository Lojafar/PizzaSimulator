using UnityEngine;

namespace Game.PizzeriaSimulator.Pizzeria.Managment.Visual
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
