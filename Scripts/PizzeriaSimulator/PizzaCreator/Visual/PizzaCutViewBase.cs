using UnityEngine;
namespace Game.PizzeriaSimulator.PizzaCreation.Visual
{
    public abstract class PizzaCutViewBase : MonoBehaviour
    {
        protected PizzaCutVM viewModel;
        protected BakedPizzaObject currentPizzaToCut;
        public virtual bool HasPizzaToCut => currentPizzaToCut != null;
        public virtual void SetPizzaToCut(BakedPizzaObject pizzaObject)
        {
            currentPizzaToCut = pizzaObject;
        }
        public virtual void Bind(PizzaCutVM _viewModel)
        {
            viewModel = _viewModel;
        }
    }
}
