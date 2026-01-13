using System.Collections.Generic;
using UnityEngine;

namespace Game.Root.Utils.Audio.Config
{
    [System.Serializable]
    class AudioData
    {
        [field:SerializeField ] public string Key { get; private set; }
        [field:SerializeField ] public AudioClip Clip { get; private set; }
    }
    [System.Serializable]
    public class AudioConfig
    {
        [SerializeField] AudioData[] audiosData;
        Dictionary<string, AudioClip> clipByKey;
        public void Init()
        {
            clipByKey = new Dictionary<string, AudioClip>();
            foreach(AudioData audioData in audiosData)
            {
                clipByKey.Add(audioData.Key, audioData.Clip);
            }
        }
        public bool TryGetClipByKey(string key, out AudioClip clip)
        {
            return clipByKey.TryGetValue(key, out clip);
        }
    }
}
