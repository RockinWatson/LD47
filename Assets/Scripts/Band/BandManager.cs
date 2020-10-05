using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts;

public class BandManager : MonoBehaviour
{
    [SerializeField] private BandMember[] _bandMembers = new BandMember[4];
    [SerializeField] private BandMemberType[] _memberPriority = new BandMemberType[4]; //@TODO: Priority of members - drums, bass, chords, melody

    [SerializeField] private GameObject _explosionPrefab = null;

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

    private void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.O))
        {
            AttackRandomAttackingFan();
        }
#endif
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
                {
                    //@TODO: Try to handle Excellent, then have it fall through... Might need to recurse down the line?
                    var bandMember = AttackRandomAttackingFan();
                    if (bandMember != null)
                    {
                        //@TODO: Boost this to next tier or something?
                        bandMember.SetPlaying();
                    }
                    else
                    {
                        //@TODO: Boost this to next tier or something?
                        StartRandomMemberPlaying();
                    }
                }
                break;
            case SequenceNote.HitScore.GOOD:
                {
                    var bandMember = AttackRandomAttackingFan();
                    if(bandMember != null)
                    {
                        bandMember.SetPlaying();
                    }
                    else
                    {
                        StartRandomMemberPlaying();
                    }
                }
                break;
            case SequenceNote.HitScore.OKAY:
                StartRandomMemberPlaying();
                break;
            case SequenceNote.HitScore.BAD:
                //@TODO: Reduce tier?
                break;
            case SequenceNote.HitScore.MISSED:
                //@TODO: Stun random?
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

    private BandMember AttackRandomAttackingFan()
    {
        var fan = FanManager.Get().GetRandomAttackingAndAliveFan();
        if(fan != null)
        {
            BandMember targetBandMember = fan.BandMemberTarget;

            var fans = FanManager.Get().GetAllNearbyAliveEnemiesAroundPos(fan.transform.position, 4f);
            if(fans != null && fans.Count > 0)
            {
                //@TODO: Add impulse to all these fuckers from nearest band member position.
                foreach(var targetFan in fans)
                {
                    Vector3 force = (targetFan.transform.position - targetBandMember.transform.position).normalized * 500f;
                    targetFan.ApplyDeathForce(targetBandMember.transform.position, force);
                }
            }

            CreateExplosionAtPos(fan.transform.position);
            Vector3 force2 = (fan.transform.position - targetBandMember.transform.position).normalized * 500f;
            fan.ApplyDeathForce(targetBandMember.transform.position, force2);

            return targetBandMember;
        }
        return null;
    }

    private void CreateExplosionAtPos(Vector3 pos)
    {
        Instantiate(_explosionPrefab, pos, Quaternion.identity, this.transform);
    }
}
