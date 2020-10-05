using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    private bool _select() { return (Input.GetKeyDown(KeyCode.Space)); }
    private bool _continue;
    public static AudioSource titleMusic;
    public static AudioSource startGame;

    // Start is called before the first frame update
    void Awake()
    {
        InitAudio();
    }

    // Update is called once per frame
    void Update()
    {
        if (_continue == false && _select())
        {
            StartCoroutine(LoadStory());
        }
        
    }

    IEnumerator LoadStory()
    {
        startGame.Play();
        titleMusic.Stop();
        _continue = true;
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("Story");
    }

    private void InitAudio()
    {

        //Initialize audio components
        AudioSource[] audio = GetComponents<AudioSource>();
        titleMusic = audio[0];
        startGame = audio[1];

        titleMusic.Play();
        titleMusic.loop = true;

    }
}
