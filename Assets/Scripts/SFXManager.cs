using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{

    public static AudioSource crowd1;
    public static AudioSource crowd2;



    // Start is called before the first frame update
    void Awake()
    {
        InitAudio();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        private void InitAudio()
        {

            //Initialize audio components
            AudioSource[] audio = GetComponents<AudioSource>();
            crowd1 = audio[0];
            crowd2 = audio[1];

            crowd1.playOnAwake = true;
            crowd1.loop = true;
            crowd2.playOnAwake = true;
            crowd2.loop = true;

            crowd1.volume = .2f;
            crowd2.volume = .2f;

        }
    }
