using TMPro;
using UnityEngine;

namespace Game.Helps.UI
{
    [RequireComponent(typeof(TMP_Text))]
    class FPSCounter : MonoBehaviour
    {
        TMP_Text tmp;
        float lastUpdateTime;
        float deltaTime;
        const float updateThreshold = 0.3f;
        private void Awake()
        {
            tmp = GetComponent<TMP_Text>();
        }
        private void Update()
        {
            lastUpdateTime += Time.deltaTime;
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            if (lastUpdateTime > updateThreshold) 
            {
                tmp.text = ((int)(1f / deltaTime)).ToString();
                lastUpdateTime = 0;
            }
        }
    }
}
