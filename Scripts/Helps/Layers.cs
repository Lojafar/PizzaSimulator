using UnityEngine;

namespace Game.Helps
{
    public static class Layers
    {
        public readonly static string DefaultLayerName = "Default";
        public readonly static string InteractableLayerName = "Interactable";

        public readonly static int DefaultLayerMask = LayerMask.GetMask(DefaultLayerName);
    }
}
