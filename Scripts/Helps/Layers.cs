using UnityEngine;

namespace Game.Helps
{
    public static class Layers
    {
        public readonly static string DefaultLayerName = "Default";
        public readonly static string InteractableLayerName = "Interactable";
        public readonly static string ObstaclesLayerName = "Obstacle";

        public readonly static int DefaultLayerMask = LayerMask.GetMask(DefaultLayerName);
    }
}
