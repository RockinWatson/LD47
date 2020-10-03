using UnityEngine;

namespace Assets.Scripts
{
    [System.Serializable]
    public class Sound
    {
        public AudioManager.Music Name;

        public AudioClip Clip;

        [Range(0f,1f)]
        public float Volume;
        public float Pitch;
        public bool Loop;

        [HideInInspector]
        public AudioSource Source;
    }
}
