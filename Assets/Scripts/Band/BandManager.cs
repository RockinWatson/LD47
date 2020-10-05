using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void SequenceCompleted(SequenceNote.HitScore hitScore)
    {
        //@TODO: React appropriately to the level of sequence completed.
        switch(hitScore)
        {
            case SequenceNote.HitScore.EXCELLENT:
                //@TODO: Try to handle Excellent, then have it fall through... Might need to recurse down the line?
            case SequenceNote.HitScore.GOOD:
            case SequenceNote.HitScore.OKAY:
                StartRandomMemberPlaying();
                break;
            case SequenceNote.HitScore.BAD:
                break;
            case SequenceNote.HitScore.MISSED:
                break;
            case SequenceNote.HitScore.NOT_HIT:
                Debug.LogWarning("This shouldn't happen - shouldn't get NOT HIT at this point.");
                break;
        }
    }

    private void StartRandomMemberPlaying()
    {
        var member = GetRandomMemberNotPlaying();
        if(member != null)
        {
            member.SetPlaying();
        }
    }

    private BandMember GetRandomMemberNotPlaying()
    {
        return _bandMembers.Where((a) => a.IsNotPlaying()).FirstOrDefault();
    }
}
