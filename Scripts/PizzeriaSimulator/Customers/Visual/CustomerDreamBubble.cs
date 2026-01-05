using UnityEngine;
using UnityEngine.UI;

namespace Game.PizzeriaSimulator.Customers.Visual
{
    public class CustomerDreamBubble : MonoBehaviour
    {
        [SerializeField] Image itemImage;
        public void SetItemSprite(Sprite sprite)
        {
            itemImage.sprite = sprite;
        }
    }
}
