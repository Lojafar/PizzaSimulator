using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PizzeriaSimulator.Customers.Visual
{
    public class CustomerDreamBubble : MonoBehaviour
    {
        [SerializeField] Image itemImage;
        readonly List<Image> itemImages = new ();
        int itemImagesInUse;
        private void Awake()
        {
            itemImages.Add(itemImage);
        }
        public void AddItemSprite(Sprite sprite)
        {
            if(itemImages.Count < itemImagesInUse + 1)
            {
                itemImages.Add(Instantiate(itemImage, itemImage.transform.parent));
            }
            itemImages[itemImagesInUse].gameObject.SetActive(true);
            itemImages[itemImagesInUse].sprite = sprite;
            itemImagesInUse++;
        }
        public void ClearItemSprites()
        {
            itemImagesInUse=0;
            for (int i = 0; i < itemImages.Count; i++)
            {
                itemImages[i].gameObject.SetActive(false);
            }
        }
    }
}
