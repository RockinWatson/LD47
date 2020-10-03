using UnityEngine;

namespace Assets.Scripts
{
    public static class SoundManager
    {
        public enum Sound
        { 
            Drums,
            KeysOne,
            KeysTwo,
            Bass
        }

        public static void PlaySound(Sound sound) {
            GameObject soundGameObject = new GameObject("Sound");
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(sound);
            audioSource.Play();
        }

        private static AudioClip GetAudioClip(Sound sound)
        {
            foreach (var soundClip in GameAssets.i.SoundClip)
            {
                if (soundClip.Sound == sound)
                {
                    return soundClip.AudioClip;
                }
            }
            Debug.LogError("Sound " + sound + " not found!");
            return null;
        }
    }
}
