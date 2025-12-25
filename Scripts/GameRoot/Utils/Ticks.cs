using System;
using UnityEngine;

namespace Game.Root.Utils
{
    class Ticks : MonoBehaviour
    {
        public event Action OnTick;
        public event Action OnFixedTick;
        public static Ticks Instance => instance;
        static Ticks instance;
        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Update()
        {
            OnTick?.Invoke();
        }
        private void FixedUpdate()
        {
            OnFixedTick?.Invoke();
        }
    }
}
