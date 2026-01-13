using Game.Root.Utils.Audio.Config;
using UnityEngine;

namespace Game.Root.Utils.Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] AudioSource sfxAudioSource;
        static AudioPlayer instance;
        static AudioConfig audioConfig;
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
        public static void SetAudioConfig(AudioConfig _audioConfig)
        {
            if (_audioConfig == null) return;
            audioConfig = _audioConfig;
        }
        public static void PlaySFX(AudioClip clip)
        {
            if (instance == null || clip == null) return;
            instance.sfxAudioSource.PlayOneShot(clip);
        }
        public static void PlaySFX(AudioClip clip, float volumeModifier)
        {
            if (instance == null || clip == null) return;
            instance.sfxAudioSource.PlayOneShot(clip, volumeModifier);
        }
        public static void PlaySFX(string clipKey)
        {
            if (instance == null || audioConfig == null) return; 
            if (audioConfig.TryGetClipByKey(clipKey, out AudioClip clip))
            {
                instance.sfxAudioSource.PlayOneShot(clip);
            }
        }
        public static void PlaySFX(string clipKey, float volumeModifier)
        {
            if (instance == null || audioConfig == null) return;
            if(audioConfig.TryGetClipByKey(clipKey, out AudioClip clip))
            {
                instance.sfxAudioSource.PlayOneShot(clip, volumeModifier);
            }
        }
    }
}
