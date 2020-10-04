using Assets.Scripts;
using UnityEngine;

public class CrowdMember : MonoBehaviour
{
    public float MoveSpeed;
    public GameObject[] BandMembers;

    private Vector3 _bandPosition;

    private void Start()
    {
        _bandPosition = GetRandomBandMemberObject();
    }

    public void Update()
    {
        MoveToBandMember();
    }

    private void MoveToBandMember()
    {
        var step = CalcMoveSpeed();

        transform.position = Vector3.MoveTowards(transform.position, _bandPosition, step);
    }

    private float CalcMoveSpeed()
    {
        return MoveSpeed * Time.deltaTime;
    }

    private Vector3 GetRandomBandMemberObject()
    {
        var gameObject = BandMembers[Random.Range(0,BandMembers.Length)];
        return gameObject.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var instrument = collision.gameObject.GetComponent<BandProperties>().Instrument[0];

        //Bass Collision
        if (instrument == AudioManager.Music.Bass)
        {
            FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Bass);
        }

        //Chords Collision
        if (instrument == AudioManager.Music.Chords)
        {
            FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Chords);
        }

        //Drums Collision
        if (instrument == AudioManager.Music.Drums)
        {
            FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Drums);
        }

        //LeadMelody  Collision
        if (instrument == AudioManager.Music.Lead)
        {
            FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Lead);
            FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Melody);
        }
    }
}
