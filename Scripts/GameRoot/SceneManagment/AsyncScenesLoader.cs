using System;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using Game.Root.Utils;

namespace Game.Root.SceneManagment
{
    class AsyncScenesLoader : IScenesLoader
    {
        public event Action OnStartLoadScene;
        public event Action<float> OnSceneLoadPercentChanged;
        public event Action OnSceneLoaded;
        readonly Coroutines coroutines;
        const float fullLoadPercent = 1f;
        public AsyncScenesLoader(Coroutines _coroutines)
        {
            coroutines = _coroutines;
        }
        public void LoadScene(string sceneName)
        {
            coroutines.StartCoroutine(SceneLoadRoutine(sceneName));
        }
        IEnumerator SceneLoadRoutine(string sceneName)
        {
            OnStartLoadScene?.Invoke();
            AsyncOperation sceneLoading = SceneManager.LoadSceneAsync(sceneName);
            Time.timeScale = 1;
            while (!sceneLoading.isDone)
            {
                yield return null;
                OnSceneLoadPercentChanged?.Invoke(sceneLoading.progress);
            }
            OnSceneLoadPercentChanged?.Invoke(fullLoadPercent);
            OnSceneLoaded?.Invoke();
        }
    }
}
