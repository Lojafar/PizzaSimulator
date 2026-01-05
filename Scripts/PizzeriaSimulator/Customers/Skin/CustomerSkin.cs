using UnityEngine;
namespace Game.PizzeriaSimulator.Customers.Skin
{
    public class CustomerSkin : MonoBehaviour
    {
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public Transform HeadBone { get; private set; }
        [field: SerializeField] public Transform HandBone { get; private set; }
        [field: SerializeField] public Transform OrderPoint { get; private set; }
    }
}
