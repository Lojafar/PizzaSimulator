using UnityEngine;

namespace Game.PizzeriaSimulator.PizzaHold.Visual
{
    class PizzaBox : MonoBehaviour
    {
        [SerializeField] SpriteRenderer pizzaIconRenderer;
        public void SetPizzaIcon(Sprite icon)
        {
            pizzaIconRenderer.sprite = icon;
        }
    }
}
