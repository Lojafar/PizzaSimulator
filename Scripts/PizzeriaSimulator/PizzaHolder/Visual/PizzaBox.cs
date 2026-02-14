using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaHold.Visual
{
    public sealed class PizzaBox : MonoBehaviour
    {
        public int PizzaID { get; private set; } = -1;
        [SerializeField] SpriteRenderer pizzaIconRenderer; 
        public void SetPizzaId(int id)
        {
            if (PizzaID != -1) return;
            PizzaID = id;
        }
        public void SetPizzaIcon(Sprite icon)
        {
            pizzaIconRenderer.sprite = icon;
        }
    }
}
