using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        private readonly Dictionary<AudioManager.Music, bool> _canTurnOnTrackDict = new Dictionary<AudioManager.Music, bool>();

        private void Awake()
        {
            foreach (AudioManager.Music music in Enum.GetValues(typeof(AudioManager.Music)))
            {
                _canTurnOnTrackDict.Add(music, false);
            }
        }

        public void UpdateCanTurnOnTrack(AudioManager.Music track, bool canTurnOn)
        {
            _canTurnOnTrackDict[track] = canTurnOn;
        }

        private void Update()
        {
            ReadInputs();
        }

        private void ReadInputs()
        {
            //Bass Controller
            if (Input.GetKeyDown(KeyCode.Alpha1) && _canTurnOnTrackDict[AudioManager.Music.Bass].Equals(true))
            {
                FindObjectOfType<AudioManager>().UnMute(AudioManager.Music.Bass);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Bass);
            }

            //Chords Controller
            if (Input.GetKeyDown(KeyCode.Alpha2) && _canTurnOnTrackDict[AudioManager.Music.Chords].Equals(true))
            {
                FindObjectOfType<AudioManager>().UnMute(AudioManager.Music.Chords);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Chords);
            }

            //Drums Controller
            if (Input.GetKeyDown(KeyCode.Alpha3) && _canTurnOnTrackDict[AudioManager.Music.Drums].Equals(true))
            {
                FindObjectOfType<AudioManager>().UnMute(AudioManager.Music.Drums);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Drums);
            }

            //LeadMelody Controller
            if (Input.GetKeyDown(KeyCode.Alpha4) && _canTurnOnTrackDict[AudioManager.Music.Lead].Equals(true))
            {
                FindObjectOfType<AudioManager>().UnMute(AudioManager.Music.Lead);
                FindObjectOfType<AudioManager>().UnMute(AudioManager.Music.Melody);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Lead);
                FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Melody);
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                FindObjectOfType<AudioManager>().PlayFullBandReverse();
            }
        }
    }
}
