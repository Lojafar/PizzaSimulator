using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.PizzeriaSimulator.OrdersHandle.Visual
{
    class OrderBar : MonoBehaviour
    {
        [SerializeField] Transform ingredientsContainer;
        [SerializeField] Image ingredientImagePrefab;
        [SerializeField] Image orderIconImage;
        [SerializeField] GameObject bakeIndicator;
        [SerializeField] GameObject cutIndicator;
        [SerializeField] GameObject completedVisual;
        [SerializeField] Color notCompletedColor = Color.white;
        [SerializeField] Color completedColor = Color.white;

        readonly List<Image> ingredients = new(ingredientAverageAmount);
        const int ingredientAverageAmount = 5;
        public void SetOrderIcon(Sprite sprite)
        {
            orderIconImage.sprite = sprite;
        }
        public void AddIngredient(Sprite sprite)
        {
            Image spawnedImage = Instantiate(ingredientImagePrefab, ingredientsContainer);
            spawnedImage.sprite = sprite;
            ingredients.Add(spawnedImage);
        }
        public void SetCompletedIngredient(int ingredientIndex, bool completed)
        {
            if (ingredients.Count > ingredientIndex)
            {
                ingredients[ingredientIndex].color = completed ? completedColor : notCompletedColor;
            }
        }
        public void DecompleteAllIngredient()
        {
            foreach (Image ingredientImage in ingredients)
            {
                ingredientImage.color = notCompletedColor;
            }
        }
        public void SetBakeIndicator()
        {
            DestroyAllIngredients();
            bakeIndicator.SetActive(true);
            cutIndicator.SetActive(false);
        }
        public void SetCutIndicator()
        {
            DestroyAllIngredients();
            bakeIndicator.SetActive(false);
            cutIndicator.SetActive(true);
        }
        public void SetAsCompleted()
        {
            DestroyAllIngredients();
            bakeIndicator.SetActive(false);
            cutIndicator.SetActive(false);
            completedVisual.SetActive(true);
        }
        void DestroyAllIngredients()
        {
            for (int i = 0; i < ingredients.Count; i++)
            {
                Destroy(ingredients[i].gameObject);
            }
            ingredients.Clear();
        }
    }
}
