using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class AudioManager : MonoBehaviour
    {
        public Sound[] Sounds;

        public enum Music { 
            FullBand,
            Bass,
            Chords,
            Drums,
            Lead,
            Melody
        }

        private void Awake()
        {
            //Create each Sound Object in Audio Manager.
            foreach (Sound s in Sounds)
            {
                s.Source = gameObject.AddComponent<AudioSource>();
                s.Source.clip = s.Clip;
                s.Source.volume = s.Volume;
                s.Source.loop = s.Loop;
            }
        }

        private void Start()
        {
            foreach (Music music in Enum.GetValues(typeof(Music)))
            {
                if (music == Music.FullBand){ continue; }
                Play(music);
            }
        }

        private Sound GetSound(Music soundName)
        {
            Sound s = Array.Find(Sounds, sound => sound.Name == soundName);
            return s;
        }

        public void Play(Music soundName)
        {
            GetSound(soundName).Source.Play();
        }

        public void Stop(Music soundName)
        {
            GetSound(soundName).Source.Stop();
        }

        public void Mute(Music soundName)
        {
            GetSound(soundName).Source.volume = 0f;
        }

        public void UnMute(Music soundName)
        {
            GetSound(soundName).Source.volume = 1f;
        }

        public float GetVolume(Music soundName)
        {
            return GetSound(soundName).Source.volume;
        }

        public void PlayFullBandReverse()
        {
            var fullBand = GetSound(Music.FullBand);
            fullBand.Source.pitch = -5;
            fullBand.Source.loop = true;
            fullBand.Source.Play();
            StartCoroutine(StopLoop(fullBand));
        }

        public IEnumerator StopLoop(Sound sound)
        {
            yield return new WaitForSeconds(1f);
            sound.Source.loop = false;
        }
    }
}
