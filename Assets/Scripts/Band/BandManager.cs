using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class BandManager : MonoBehaviour
{
    [SerializeField] private BandMember[] _bandMembers = new BandMember[4];
    [SerializeField] private BandMemberType[] _memberPriority = new BandMemberType[4]; //@TODO: Priority of members - drums, bass, chords, melody

    static private BandManager _instance = null;
    static public BandManager Get() { return _instance; }

    public enum BandMemberType
    {
        BILLY = 0,
        CACTUS = 1,
        GATOR = 2,
        CHICK = 3,
        COUNT,
    }

    public enum BandMemberStatus
    {
        NOT_PLAYING = 0,
        STUNNED = 1,
        PLAYING = 2,
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("Something fucky happened - should only be 1 BandManager");
            return;
        }

        _instance = this;
    }

    static public AudioManager.Music GetMusicByMemberType(BandMemberType memberType)
    {
        switch(memberType)
        {
            case BandMemberType.BILLY:
                return AudioManager.Music.Drums;
            case BandMemberType.CACTUS:
                return AudioManager.Music.Chords;
            case BandMemberType.GATOR:
                return AudioManager.Music.Melody;
            case BandMemberType.CHICK:
                return AudioManager.Music.Bass;
            default:
                Debug.LogError("Unkown Band Member Type");
                return AudioManager.Music.Drums;
        }
    }

    static public BandMemberType GetRandomBandMemberType()
    {
        return (BandMemberType)Random.Range(0, (int)BandMemberType.COUNT);
    }

    public BandMember GetRandomBandMember()
    {
        return _bandMembers[Random.Range(0, (int)BandMemberType.COUNT)];
    }
}
