using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.PizzeriaSimulator.UI.NewLevelPopUp.Visual
{
    class LevelRewardSlot : MonoBehaviour
    {
        [SerializeField] Image iconImage;
        [SerializeField] TMP_Text descriptionText;
        public void SetIcon(Sprite icon)
        {
            iconImage.sprite = icon;
        }
        public void SetDescription(string description)
        {
            descriptionText.text = description;
        }
    }
}
