using Game.Root.Utils.Toast;
using UnityEngine;
namespace Game.Root.Utils
{
    public class Toasts : MonoBehaviour
    {
        [SerializeField] ToastObjectBase toastPrefab;
        [SerializeField] Transform toastsContainer;
        [SerializeField] float toastDuration;
        static Toasts instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public static void ShowToast(string message)
        {
            if (instance == null) return;
            instance.SpawnAndShowToast(message);
        }
        void SpawnAndShowToast(string message)
        {
            ToastObjectBase spawnedToast = Instantiate(toastPrefab, toastsContainer);
            spawnedToast.Show(message);
            Destroy(spawnedToast, toastDuration);
        }
    }
}
