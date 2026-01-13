using Game.PizzeriaSimulator.BoxCarry.Box;
using Game.PizzeriaSimulator.Interactions;
using UnityEngine;

namespace Game.PizzeriaSimulator.BoxCarry.Carrier.Handler
{
    interface IBoxesHandler
    {
        public void SetBox(ICarriableBox box);
        public void HandleInteraction(InteractableType interactableType, GameObject interactedObject);
    }
}
