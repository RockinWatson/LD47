using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class AudioManager : MonoBehaviour
    {
        public Sound[] Sounds;

        private void Awake()
        {
            foreach (Sound s in Sounds)
            {
                s.Source = gameObject.AddComponent<AudioSource>();
                s.Source.clip = s.Clip;
                s.Source.volume = s.Volume;
            }
        }

        private void Start()
        {
            Play("FullBand");
        }

        public void Play(string soundName)
        {
            Sound s = Array.Find(Sounds, sound => sound.Name == soundName);
            s.Source.Play();
        }
    }
}
