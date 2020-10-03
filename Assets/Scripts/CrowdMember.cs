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
        //Bass Collision
        if (collision.gameObject.GetComponent<BandProperties>().Instrument == AudioManager.Music.Bass)
        {
            FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Bass);
        }

        //Chords Collision
        if (collision.gameObject.GetComponent<BandProperties>().Instrument == AudioManager.Music.Chords)
        {
            FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Chords);
        }

        //Drums Collision
        if (collision.gameObject.GetComponent<BandProperties>().Instrument == AudioManager.Music.Drums)
        {
            FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Drums);
        }

        //Lead Collision
        if (collision.gameObject.GetComponent<BandProperties>().Instrument == AudioManager.Music.Lead)
        {
            FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Lead);
        }

        //Melody Collision
        if (collision.gameObject.GetComponent<BandProperties>().Instrument == AudioManager.Music.Melody)
        {
            FindObjectOfType<AudioManager>().Mute(AudioManager.Music.Melody);
        }
    }
}
