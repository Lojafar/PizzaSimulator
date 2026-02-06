using UnityEngine;

namespace Game.PizzeriaSimulator.Boxes.Item
{
    public abstract class FurnitureBoxItemBase : MonoBehaviour
    {
        [SerializeField] protected float staticYPos;
        [field: SerializeField] public Vector3 HalfSize { get; protected set; }
        [SerializeField] protected Vector3 startRotation;
        protected virtual void Start()
        {
            transform.eulerAngles = startRotation;
        }
        public abstract void SetAsCanBePlaced();
        public abstract void SetAsCantBePlaced();
        public abstract bool CheckWallsCollision();
        public virtual void SetPos(Vector3 pos)
        {
            transform.position = new Vector3(pos.x, staticYPos, pos.z);
        }
    }
}
