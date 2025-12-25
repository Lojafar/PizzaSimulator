using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Game.Helps.UI
{
    [RequireComponent(typeof(RectTransform))]
    class CollapsibleHorizontalGroup : MonoBehaviour
    {
        [SerializeField] float moveDuration;
        [SerializeField] float spacing;
        [SerializeField] float leftOffset;
        [SerializeField] bool ignoreLastElement;
        readonly List<RectTransform> elements = new();
        readonly Vector2 elementsAnchor = new(0, 0.5f);
        bool expanded = true;
        int childChangeIgnors;
        Sequence currentAnim;
        float StartPosOffset => leftOffset - spacing;
        private void Awake()
        {
            OnTransformChildrenChanged();
        }
        private void OnTransformChildrenChanged()
        {
            if (childChangeIgnors > 0)
            {
                childChangeIgnors--;
                return;
            }
            elements.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i) is RectTransform childRT)
                {
                    elements.Add(childRT); 
                }
                else
                {
                    Debug.LogError("Child isn't a rect transform!");
                    return;
                }
            }
            UpdateChilds();
        }
        void UpdateChilds()
        {
            if (currentAnim.IsActive())
            {
                currentAnim.Kill();
            }
            float lastPosX = StartPosOffset;
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].anchorMin = elementsAnchor;
                elements[i].anchorMax = elementsAnchor;
                if (expanded || i == 0 || (ignoreLastElement && i == elements.Count - 1))
                {
                    lastPosX = CalcElementPosX(elements[i], lastPosX);
                    elements[i].anchoredPosition = new Vector2(lastPosX, 0);
                    lastPosX += elements[i].sizeDelta.x / 2;
                }
                else
                {
                    elements[i].gameObject.SetActive(false);
                }
            }
        }
        public void Compress()
        {
            if (!expanded) return;
            currentAnim = null;
            if (currentAnim.IsActive())
            {
                currentAnim.Kill();
            }
            expanded = false;
            currentAnim = DOTween.Sequence();
            for (int i = 1; i < elements.Count; i++)
            {
                if (ignoreLastElement && i == elements.Count - 1) 
                {
                    currentAnim.Join(elements[i].DOAnchorPos
                        (new Vector2(CalcElementPosX(elements[i], leftOffset + elements[0].sizeDelta.x) , 0),
                        moveDuration).SetEase(Ease.Linear));
                }
                else
                {
                    int currentI = i;
                    currentAnim.Join(elements[i].DOAnchorPos(elements[0].anchoredPosition, moveDuration).
                        SetEase(Ease.Linear).OnComplete(() => elements[currentI].gameObject.SetActive(false)));
                }
            }
            currentAnim.Play();
        }
        public void Expand()
        {
            if(expanded) return;
            if (currentAnim.IsActive())
            {
                currentAnim.Kill();
            }
            expanded = true;
            float lastPosX = StartPosOffset;
            currentAnim = DOTween.Sequence();
            for (int i = 0; i < elements.Count; i++)
            {
                if (!ignoreLastElement || i != elements.Count - 1)
                {
                    elements[i].anchoredPosition = elements[0].anchoredPosition;
                }
                elements[i].gameObject.SetActive(true);
                lastPosX = CalcElementPosX(elements[i], lastPosX);
                currentAnim.Join(elements[i].DOAnchorPos(new Vector2(lastPosX, 0), moveDuration).SetEase(Ease.Linear));
                lastPosX += elements[i].sizeDelta.x / 2;
            }
            currentAnim.Play();
        }
        float CalcElementPosX(RectTransform element, float lastPosX)
        {
            return lastPosX + element.sizeDelta.x / 2 + spacing;
        }
        public void AddChildChangeIgnors(int times = 1)
        {
            if (times < 1) return;
            childChangeIgnors += times;
        }
    }
}
