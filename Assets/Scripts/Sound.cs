using UnityEngine;

namespace Assets.Scripts
{
    [System.Serializable]
    public class Sound
    {
        public string Name;

        public AudioClip Clip;

        [Range(0f,1f)]
        public float Volume;
        public float Pitch;

        [HideInInspector]
        public AudioSource Source;
    }
}
