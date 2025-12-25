using UnityEngine;

namespace Game.Root.Utils.Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField] AudioSource sfxAudioSource;
        static AudioPlayer instance;
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
    }
}
