using Game.Helps.UI;
using Game.Root.Utils;
using Game.Root.Utils.Audio;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PizzeriaSimulator.Orders.Handle.Visual
{
    class PizzeriaOrderHandlerUIView : PizzeriaOrderHandlViewBase
    {
        [SerializeField] AudioClip bellClip;
        [SerializeField] OrderBar orderBarPrefab;
        [SerializeField] GameObject expandIndicator;
        [SerializeField] Transform expandIndicatorTransform;
        [SerializeField] CollapsibleHorizontalGroup orderBarsContainer;
        [SerializeField] OrderHandlSubViewBase pcSubView;
        [SerializeField] OrderHandlSubViewBase mobileSubView;
        [SerializeField] float expdIndicatNormalRotZ;
        [SerializeField] float expdIndicatExpandedRotZ;
        Dictionary<int, OrderBar> orderBarsDict;
        OrderHandlSubViewBase subView;
        const int minOrdersForExpdIndic = 2;
        public override void Bind(PizzeriaOrderHandlVM _viewModel)
        {
            orderBarsDict = new Dictionary<int, OrderBar>();
            base.Bind(_viewModel);
            viewModel.AddNewOrder += SpawnNewOrderBar;
            viewModel.SetOrderIndicatorIcon +=SetIndicatorInOrder;
            viewModel.SetOrderIndicatorsIcons += SetIndicatorsInOrder;
            viewModel.SetIndicDoneInOrder += DoneIndicatorInOrder;
            viewModel.ClearIndicsDonesInOrder += DecompleteIndicatsInOrder;
            viewModel.SetItemReadyInOrder += SetItemReadyInOrder;
            viewModel.SetOrderAsReady += SetBarAsCompleted;
            viewModel.CompleteOrder += CompleteOrder;
            viewModel.ShowBellCancel += OnBellCancelled;

            subView = viewModel.DeviceType == Root.User.Environment.DeviceType.PC ? pcSubView : mobileSubView;
            subView.OnExpandInput += ExpandContainer;
            subView.OnCompressInput += CompressContainer;

            orderBarsContainer.Compress();
            orderBarsContainer.AddChildChangeIgnors(1);
            expandIndicator.SetActive(false);
        }
        private void OnDestroy()
        {
            if (viewModel != null)
            { 
                viewModel.AddNewOrder -= SpawnNewOrderBar;
                viewModel.SetOrderIndicatorIcon -= SetIndicatorInOrder;
                viewModel.SetOrderIndicatorsIcons -= SetIndicatorsInOrder;
                viewModel.SetIndicDoneInOrder -= DoneIndicatorInOrder;
                viewModel.ClearIndicsDonesInOrder -= DecompleteIndicatsInOrder;
                viewModel.SetItemReadyInOrder -= SetItemReadyInOrder;
                viewModel.SetOrderAsReady -= SetBarAsCompleted;
                viewModel.CompleteOrder -= CompleteOrder;
                viewModel.ShowBellCancel -= OnBellCancelled;
                subView.OnExpandInput -= ExpandContainer;
                subView.OnCompressInput -= CompressContainer;
            }
        }
        void SpawnNewOrderBar(OrderVisualData orderVisualData)
        {
            if (orderBarsDict.Count + 1 >= minOrdersForExpdIndic)
            {
                if (!expandIndicator.activeInHierarchy)
                {
                    subView.UpdateCompress(orderBarsContainer.Expanded);
                    expandIndicatorTransform.eulerAngles = new Vector3(expandIndicatorTransform.eulerAngles.x, expandIndicatorTransform.eulerAngles.y,
                        orderBarsContainer.Expanded ? expdIndicatExpandedRotZ : expdIndicatNormalRotZ);
                }
                orderBarsContainer.AddChildChangeIgnors(1);
                expandIndicator.SetActive(true);
            }
            OrderBar spawnedBar = Instantiate(orderBarPrefab, orderBarsContainer.transform);
            expandIndicator.transform.SetAsLastSibling();
            spawnedBar.SetOrderItemsIcons(orderVisualData.OrderItemsIcons);
            orderBarsDict.Add(orderVisualData.OrderId, spawnedBar);
        }
        void SetIndicatorInOrder(int orderId, Sprite indicator)
        {
            if (orderBarsDict.TryGetValue(orderId, out OrderBar bar))
            {
                bar.SetIndicator(indicator);
            }
        }
        void SetIndicatorsInOrder(int orderId, Sprite[] indicators)
        {
            if (orderBarsDict.TryGetValue(orderId, out OrderBar bar))
            {
                bar.SetIndicators(indicators);
            }
        }
        void DoneIndicatorInOrder(int orderId, int indicatorId)
        {
            if (orderBarsDict.TryGetValue(orderId, out OrderBar bar))
            {
                bar.CompleteIndicator(indicatorId, true);
            }
        }
        void DecompleteIndicatsInOrder(int barIndex)
        {
            if (orderBarsDict.TryGetValue(barIndex, out OrderBar bar))
            {
                bar.DecompleteAllIndicators();
            }
        }
        void SetItemReadyInOrder(int orderId, int itemId)
        {
            if (orderBarsDict.TryGetValue(orderId, out OrderBar bar))
            {
                bar.CompleteItem(itemId);
            }
        }
        void SetBarAsCompleted(int barIndex)
        {
            if (orderBarsDict.TryGetValue(barIndex, out OrderBar bar))
            {
                bar.SetAsCompleted();
            }
        }
        void CompleteOrder(int barIndex)
        {
            if (orderBarsDict.TryGetValue(barIndex, out OrderBar bar))
            {
                AudioPlayer.PlaySFX(bellClip);
                Destroy(bar.gameObject);
                orderBarsDict.Remove(barIndex);
                expandIndicator.SetActive(orderBarsDict.Count >= minOrdersForExpdIndic);
            }
        }
        void OnBellCancelled(string message)
        {
            Toasts.ShowToast(message);
            AudioPlayer.PlaySFX("Wrong");
        }
        void ExpandContainer()
        {
            Vector3 expandIndicEulerRot = expandIndicatorTransform.eulerAngles;
            expandIndicEulerRot.z = expdIndicatExpandedRotZ;
            expandIndicatorTransform.eulerAngles = expandIndicEulerRot;
            orderBarsContainer.Expand(); 
            expandIndicator.SetActive(orderBarsDict.Count >= minOrdersForExpdIndic);
            subView.OnExpanded();
        }
        void CompressContainer()
        {
            Vector3 expandIndicEulerRot = expandIndicatorTransform.eulerAngles;
            expandIndicEulerRot.z = expdIndicatNormalRotZ;
            expandIndicatorTransform.eulerAngles = expandIndicEulerRot;
            orderBarsContainer.Compress();
            expandIndicator.SetActive(orderBarsDict.Count >= minOrdersForExpdIndic);
            subView.OnCompressed();
        }
    }
}
