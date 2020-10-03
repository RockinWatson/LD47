using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        private void Update()
        {
            ReadInputs();
        }

        private void ReadInputs()
        {
            //Bass Controller
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                FindObjectOfType<AudioManager>().UnMute(AudioManager.Music.Bass);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Bass);
            }

            //Chords Controller
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                FindObjectOfType<AudioManager>().UnMute(AudioManager.Music.Chords);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Chords);
            }

            //Drums Controller
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                FindObjectOfType<AudioManager>().UnMute(AudioManager.Music.Drums);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Drums);
            }

            //Lead Controller
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                FindObjectOfType<AudioManager>().UnMute(AudioManager.Music.Lead);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Lead);
            }

            //Melody Controller
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                FindObjectOfType<AudioManager>().UnMute(AudioManager.Music.Melody);
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Melody);
            }
        }
    }
}
