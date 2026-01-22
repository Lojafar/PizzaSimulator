using Game.PizzeriaSimulator.Interactions;
using UnityEngine;

namespace Game.PizzeriaSimulator.Boxes.Carry.Handler
{
    interface IBoxesHandler
    {
        public void SetBox(CarriableBoxBase box);
        public void HandleInteraction(InteractableType interactableType, GameObject interactedObject);
    }
}
