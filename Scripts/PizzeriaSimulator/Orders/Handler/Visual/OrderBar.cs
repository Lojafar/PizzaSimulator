using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PizzeriaSimulator.Orders.Handle.Visual
{
    class OrderBar : MonoBehaviour
    {
        [SerializeField] Transform indicatorsContainer;
        [SerializeField] Image orderItemIconImage;
        [SerializeField] Image indicatorImagePrefab;
        [SerializeField] GameObject completedVisual;
        [SerializeField] Color readyItemColor = Color.white;
        [SerializeField] Color notCompletedIndicColor = Color.white;
        [SerializeField] Color completedIndicColor = Color.white;

        readonly List<Image> indicators = new(indicatorsPossibleAmount);
        Image[] orderItemsImages;
        const int indicatorsPossibleAmount = 5;
        public void SetOrderItemsIcons(Sprite[] icons)
        {
            orderItemsImages = new Image[icons.Length];
            for (int i = 0; i < icons.Length; i++)
            {
                orderItemsImages[i] = i == 0? orderItemIconImage : Instantiate(orderItemIconImage, orderItemIconImage.transform.parent);
                orderItemsImages[i].sprite = icons[i];
            }
        }
        public void SetIndicator(Sprite indicatorSprite)
        {
            DestroyAllIndicators();
            SpawnIndicator(indicatorSprite);
        }
        public void SetIndicators(Sprite[] indicatorsSprites)
        {
            DestroyAllIndicators();
            for (int i = 0; i < indicatorsSprites.Length; i++)
            {
                SpawnIndicator(indicatorsSprites[i]);
            }
        }
        void DestroyAllIndicators()
        {
            for (int i = 0; i < indicators.Count; i++)
            {
                Destroy(indicators[i].gameObject);
            }
            indicators.Clear();
        }
        void SpawnIndicator(Sprite indicatorSprite)
        {
            Image spawnedImage = Instantiate(indicatorImagePrefab, indicatorsContainer);
            spawnedImage.sprite = indicatorSprite;
            indicators.Add(spawnedImage);
        }
        public void CompleteItem(int itemID)
        {
            if (orderItemsImages.Length > itemID)
            {
                orderItemsImages[itemID].color = readyItemColor;
                if (orderItemsImages[itemID].transform.childCount > 0) orderItemsImages[itemID].transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        public void CompleteIndicator(int indicatorIndex, bool completed)
        {
            if (indicators.Count > indicatorIndex)
            {
                indicators[indicatorIndex].color = completed ? completedIndicColor : notCompletedIndicColor;
            }
        }
        public void DecompleteAllIndicators()
        {
            foreach (Image ingredientImage in indicators)
            {
                ingredientImage.color = notCompletedIndicColor;
            }
        }
        public void SetAsCompleted()
        {
            if(completedVisual != null)
            completedVisual.SetActive(true);
        }
    }
}
