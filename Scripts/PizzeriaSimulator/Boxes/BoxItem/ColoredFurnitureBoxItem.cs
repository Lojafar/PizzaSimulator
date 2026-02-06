using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.PizzeriaSimulator.Boxes.Item
{
    class ColoredFurnitureBoxItem : FurnitureBoxItemBase
    {
        [SerializeField] Material[] canBePlacedMats;
        [SerializeField] Material[] cantBePlacedMats;
        [SerializeField] MeshRenderer[] meshRenderers;
        Collider[] overlapResult;
        private void Awake()
        {
            overlapResult = new Collider[1];
        }
        protected async override void Start()
        {
            base.Start();
            await UniTask.Yield();
        }
        public override void SetAsCanBePlaced()
        {
            if (canBePlacedMats == null) return;
            ChangeMatAllRenderers(canBePlacedMats);
        }
        public override void SetAsCantBePlaced()
        {
            if (cantBePlacedMats == null) return;
            ChangeMatAllRenderers(cantBePlacedMats);
        }
    
        void ChangeMatAllRenderers(Material[] materials)
        {
            if (meshRenderers == null) return;
            for(int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].sharedMaterials = materials; 
            }
        }
        public override bool CheckWallsCollision()
        {
            return Physics.OverlapBoxNonAlloc(transform.position, HalfSize, overlapResult, transform.rotation) > 0;
        }
    }
}
