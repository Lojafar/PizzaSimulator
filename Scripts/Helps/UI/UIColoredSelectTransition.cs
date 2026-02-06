using UnityEngine;
using UnityEngine.UI;
namespace Game.Helps.UI
{
    sealed class UIColoredSelectTransition : UISelectTransitionBase
    {
        [SerializeField] Graphic[] graphicsToChange;
        [SerializeField] Color[] selectedColors;
        [SerializeField] Color[] deselectedColors;
        private void Awake()
        {
            if(graphicsToChange.Length != selectedColors.Length)
            {
                UnityEngine.Debug.LogError("SelectedColors length isn't equal to graphicsToChange length");
                selectedColors = new Color[graphicsToChange.Length];
            }
            if (graphicsToChange.Length != deselectedColors.Length)
            {
                UnityEngine.Debug.LogError("DeselectedColors length isn't equal to graphicsToChange length");
                deselectedColors = new Color[graphicsToChange.Length];
            }
        }
        public override void Select()
        {
            for (int i = 0; i < graphicsToChange.Length; i++)
            {
                graphicsToChange[i].color = selectedColors[i];
            }
        }
        public override void Deselect()
        {
            for (int i = 0; i < graphicsToChange.Length; i++)
            {
                graphicsToChange[i].color = deselectedColors[i];
            }
        }
      
    }
}
