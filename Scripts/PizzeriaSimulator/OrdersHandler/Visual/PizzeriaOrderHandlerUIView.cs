using UnityEngine;
using System.Collections.Generic;
using Game.Root.Utils;
using Game.Root.Utils.Audio;
using Game.Helps.UI;

namespace Game.PizzeriaSimulator.OrdersHandle.Visual
{
    class PizzeriaOrderHandlerUIView : PizzeriaOrderHandlViewBase
    {
        [SerializeField] AudioClip bellClip;
        [SerializeField] AudioClip bellCancellClip;
        [SerializeField] OrderBar orderBarPrefab;
        [SerializeField] GameObject expandIndicator;
        [SerializeField] Transform expandIndicatorVisual;
        [SerializeField] CollapsibleHorizontalGroup orderBarsContainer;
        [SerializeField] OrderHandlSubViewBase pcSubView;
        [SerializeField] OrderHandlSubViewBase mobileSubView;
        [SerializeField] float expIndicatNormalRotZ;
        [SerializeField] float expIndicatExpandedRotZ;
        Dictionary<int, OrderBar> orderBars;
        OrderHandlSubViewBase subView;
        const int minOrdersForExpIndic = 2;
        public override void Bind(PizzeriaOrderHandlVM _viewModel)
        {
            orderBars = new Dictionary<int, OrderBar>();
            base.Bind(_viewModel);
            viewModel.OnNewOrder += SpawnNewOrderBar;
            viewModel.ComleteIngredientInOrder += CompleteIngredientOnBar;
            viewModel.ClearAllCompletedInBar += ClearAllCompletesInBar;
            viewModel.ClearAllCompletedInBars += ClearCompletesInAllBars;
            viewModel.SetOrderBakeState += SetBarBakeState;
            viewModel.SetOrderCutState += SetBarCutState;
            viewModel.SetOrderCompleted += SetBarAsCompleted;
            viewModel.CompleteOrder += CompleteOrder;
            viewModel.OnBellCancelled += OnBellCancelled;
            subView = viewModel.DeviceType == Root.User.Environment.DeviceType.PC ? pcSubView : mobileSubView;
            subView.OnExpandInput += ExpandContainer;
            subView.OnCompressInput += CompressContainer;
            expandIndicator.SetActive(false);
            CompressContainer();
        }
        private void OnDestroy()
        {
            if (viewModel != null)
            { 
                viewModel.OnNewOrder -= SpawnNewOrderBar;
                viewModel.ComleteIngredientInOrder -= CompleteIngredientOnBar;
                viewModel.ClearAllCompletedInBar -= ClearAllCompletesInBar;
                viewModel.ClearAllCompletedInBars -= ClearCompletesInAllBars;
                viewModel.SetOrderBakeState -= SetBarBakeState;
                viewModel.SetOrderCutState -= SetBarCutState;
                viewModel.SetOrderCompleted -= SetBarAsCompleted;
                viewModel.CompleteOrder -= CompleteOrder;
                viewModel.OnBellCancelled -= OnBellCancelled;
                subView.OnExpandInput -= ExpandContainer;
                subView.OnCompressInput -= CompressContainer;
            }
        }
        void ExpandContainer()
        {
            expandIndicatorVisual.eulerAngles = new Vector3(expandIndicatorVisual.eulerAngles.x, expandIndicatorVisual.eulerAngles.y, expIndicatExpandedRotZ);
            orderBarsContainer.Expand(); 
            expandIndicator.SetActive(orderBars.Count >= minOrdersForExpIndic);
            subView.OnExpanded();
        }
        void CompressContainer()
        {
            expandIndicatorVisual.eulerAngles = new Vector3(expandIndicatorVisual.eulerAngles.x, expandIndicatorVisual.eulerAngles.y, expIndicatNormalRotZ);
            orderBarsContainer.Compress();
            expandIndicator.SetActive(orderBars.Count >= minOrdersForExpIndic);
            subView.OnCompressed();
        }
        void SpawnNewOrderBar(OrderVisualData orderVisualData)
        {
            if(orderBars.Count + 1 >= minOrdersForExpIndic)
            {
                orderBarsContainer.AddChildChangeIgnors(1);
                expandIndicator.SetActive(true);
            }
            OrderBar spawnedBar = Instantiate(orderBarPrefab, orderBarsContainer.transform);
            expandIndicator.transform.SetAsLastSibling();
            spawnedBar.SetOrderIcon(orderVisualData.OrderIcon);
            if (orderVisualData.InCut)
            {
                spawnedBar.SetCutIndicator();
            }
            else if (orderVisualData.InBake)
            {
                spawnedBar.SetBakeIndicator();
            }
            else
            {
                for (int i = 0; i < orderVisualData.IngredientsSprites.Length; i++)
                {
                    spawnedBar.AddIngredient(orderVisualData.IngredientsSprites[i]);
                    spawnedBar.SetCompletedIngredient(i, orderVisualData.ReadyIngredients[i]);
                }
            }
            orderBars.Add(orderVisualData.OrderId, spawnedBar);
        }
        void CompleteIngredientOnBar(int barIndex, int ingredientIndex)
        {
            if (orderBars.TryGetValue(barIndex, out OrderBar bar))
            {
                bar.SetCompletedIngredient(ingredientIndex, true);
            }
        }
        void ClearAllCompletesInBar(int barIndex)
        {
            if (orderBars.TryGetValue(barIndex, out OrderBar bar))
            {
                bar.DecompleteAllIngredient();
            }
        }
        void ClearCompletesInAllBars()
        {
            foreach(OrderBar bar in orderBars.Values)
            {
                bar.DecompleteAllIngredient();
            }
        }
        void SetBarBakeState(int barIndex)
        {
            if (orderBars.TryGetValue(barIndex, out OrderBar bar))
            {
                bar.SetBakeIndicator();
            }
        }
        void SetBarCutState(int barIndex)
        {
            if (orderBars.TryGetValue(barIndex, out OrderBar bar))
            {
                 bar.SetCutIndicator();
            }
        }
        void SetBarAsCompleted(int barIndex)
        {
            if (orderBars.TryGetValue(barIndex, out OrderBar bar))
            {
               bar.SetAsCompleted();
            }
        }
        void CompleteOrder(int barIndex)
        {
            if (orderBars.TryGetValue(barIndex, out OrderBar bar))
            {
                AudioPlayer.PlaySFX(bellClip);
                Destroy(bar.gameObject);
                orderBars.Remove(barIndex);
                expandIndicator.SetActive(orderBars.Count >= minOrdersForExpIndic);
            }
        }
        void OnBellCancelled(string message)
        {
            Toasts.ShowToast(message);
            AudioPlayer.PlaySFX(bellCancellClip);
        }
    }
}
