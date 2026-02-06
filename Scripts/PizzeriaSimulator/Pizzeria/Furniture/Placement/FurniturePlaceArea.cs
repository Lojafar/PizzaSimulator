using UnityEngine;
namespace Game.PizzeriaSimulator.Pizzeria.Furniture.Placement
{
    public class FurniturePlaceArea : MonoBehaviour
    {
        [SerializeField] protected Bounds thisBounds;
        protected Vector3 thisBoundsMin;
        protected Vector3 thisBoundsMax;
        const float magnetPosAdd = 0.03f;
        [ContextMenu("BoundsCenterToTrsformPos")]
        void BoundsCenterToTrsformPos()
        {
            thisBounds.center = transform.position;
        }
        protected virtual void Awake()
        {
            thisBoundsMin = thisBounds.min;
            thisBoundsMax = thisBounds.max;
        }
        public virtual bool IsBoundsInside(Bounds otherBounds)
        {
            Vector3 otherMin = otherBounds.min;
            Vector3 otherMax = otherBounds.max;
            otherMin.y = thisBounds.center.y;
            otherMax.y = otherMin.y;
            return thisBounds.Contains(otherMin) && thisBounds.Contains(otherMax);
        }
        public virtual bool TryGetMagnetizedPos(Bounds otherBounds, out Vector3 magnetizedPos)
        {
            magnetizedPos = otherBounds.center;
            if (!thisBounds.Intersects(otherBounds)) return false;

            sbyte xSide = 0;
            sbyte zSide = 0;

            if (Mathf.Abs(thisBoundsMin.x - otherBounds.center.x) < otherBounds.extents.x) xSide = -1;
            else if (Mathf.Abs(otherBounds.center.x - thisBoundsMax.x) < otherBounds.extents.x) xSide = 1;

            if (Mathf.Abs(thisBoundsMin.z - otherBounds.center.z) < otherBounds.extents.z) zSide = -1;
            else if (Mathf.Abs(otherBounds.center.z - thisBoundsMax.z) < otherBounds.extents.z) zSide = 1;

            if (xSide == 0 && zSide == 0)
            {
                return false;
            }

            if (xSide != 0)
            {
                magnetizedPos.x = thisBounds.center.x + ((thisBounds.extents.x - otherBounds.extents.x - magnetPosAdd) * xSide);
            }
            if (zSide != 0)
            {
                magnetizedPos.z = thisBounds.center.z + ((thisBounds.extents.z - otherBounds.extents.z - magnetPosAdd) * zSide);
            }
            return true;
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(thisBounds.center, thisBounds.size);
        }
    }
}
